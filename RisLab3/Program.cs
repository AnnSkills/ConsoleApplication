using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace RisLab3 {
    class Program {
        public const string FILE_NAME = "schedule.txt";
        public const int COLUMN_SIZE = 25;

        static void Main(string[] args) {
            Program app = new Program();
            app.StartProject();
        }

        private List<Factory> schedules = null;

        private void StartProject() {
            Console.WriteLine("                                         ПРИЛОЖЕНИЕ УЧЕТА НАГРУЗКИ ПО ПРЕДМЕТАМ");
            Console.Write("\nINFO: Данное приложение предоставляет перечень изучаемых дисциплин специальностей БГУИР\n");
            ReadData();
            CreateMenu();
            WriteData();
            Console.WriteLine("Как, вы уже уходите? Очень жаль :( ");
            Console.ReadKey();
        }

        private void CreateMenu() {
            while (true) {
                ShowMenu();
                int menuItem = Program.EnterIntNumber();
                if (!ExecuteOperation(menuItem)) {
                    break;
                }
            }
        }

        private bool ExecuteOperation(int menuItem) {
            switch (menuItem) {
                case 0:
                    return false;
                case 1:
                    this.Create(); break;
                case 2:
                    this.Read(this.schedules); break;
                case 3:
                    this.Update(); break;
                case 4:
                    this.Delete(); break;
                case 5:
                    this.Find(); break;
                case 6:
                    this.Sort(); break;
                default:
                    Console.WriteLine("Некорректный пункт меню (" + menuItem + ")!"); break;
            }
            return true;
        }


        private void ShowMenu() {
            String[] menuRows = {"\n Меню с вариантами развития событий:", "1. Добавить новую запись", "2. Просмотреть существующие записи",
                "3. Изменить выбранную запись", "4. Удалить запись по ID", "5. Поиск по записям" , "6. Сортировать записи по выбранному полю", "0. Я закончил(а) работу и хочу выйти из приложения\n Какое действие будет следующим?" };
            foreach (String menuRow in menuRows) {
                Console.WriteLine(menuRow);
            }
        }

        private void ReadData() {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Factory>));

            using (FileStream stream = new FileStream(Program.FILE_NAME, FileMode.OpenOrCreate)) {
                try {
                    object tempObject = jsonFormatter.ReadObject(stream);
                    this.schedules = (List<Factory>)tempObject;
                } catch(Exception ex) {
                    this.schedules = new List<Factory>();
                }
            }
        }

        private void WriteData() {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Factory>));

            using (FileStream stream = new FileStream(Program.FILE_NAME, FileMode.OpenOrCreate)) {
                jsonFormatter.WriteObject(stream, this.schedules);
            }
        }

        private void Read(List<Factory> schedulesToShow) {
            ShowHead();
            foreach (Factory schedule in schedulesToShow) {
                ReadOneRow(schedule);
            }
        }

        private void ShowHead() {
            for (int i = 0; i < 113; i++) { Console.Write("-"); }
            this.showOneRow("ID", "Год поступления", "Факультет", "Форма обучения", "Специальность", "Дисциплина","Часы");
            for (int i = 0; i < 113; i++) { Console.Write("-"); }
        }

        private void ReadOneRow(Factory schedule) {
            this.showOneRow(schedule.Id.ToString(), schedule.Year.ToString(), schedule.Faculty, schedule.Form, schedule.Specialty, schedule.Subject, schedule.Hour.ToString());
            for (int i = 0; i < 113; i++) { Console.Write("-"); }
        }

        private void showOneRow(string first, string second, string thrid, string fourth, string fifth, string sixth, string seventh) {
            first += getSpaces(first);
            second += getSpaces(second);
            thrid += getSpaces(thrid);
            fourth += getSpaces(fourth);
            fifth += getSpaces(fifth);
            sixth += getSpaces(sixth);
            seventh += getSpaces(seventh);
            Console.WriteLine("\n|" + first + "|" + second + "|" + thrid + "|" + fourth + "|" + fifth + "|" + sixth + "|" + seventh + "|");
        }

        private string getSpaces(string value) {
            string spaces = "";
            for (int i = 0; i < COLUMN_SIZE - value.Length-10; i++) {
                spaces += " ";
            }
            return spaces;
        }

        private void Create() {
            Factory newSchedule = new Factory();
            Console.Write("Введите ИД: ");
            newSchedule.Id = this.EnterUniqueId();
            Console.Write("Введите год поступления: ");
            newSchedule.Year = Program.EnterDoubleNumber();
            Console.Write("Введите факультет: ");
            newSchedule.Faculty = Program.EnterString();
            Console.Write("Введите форму обучения: ");
            newSchedule.Form = Program.EnterString();
            Console.Write("Введите специальность: ");
            newSchedule.Specialty = Program.EnterString();
            Console.Write("Введите дисциплину: ");
            newSchedule.Subject = Program.EnterString();
            Console.Write("Введите часы: ");
            newSchedule.Hour = Program.EnterDoubleNumber();
            this.schedules.Add(newSchedule);
        }

        private int EnterUniqueId() {
            int id;
            while (true) {
                id = Program.EnterIntNumber();
                if (this.isIdUnique(id)) {
                    break;
                } else {
                    Console.Write("Ид не уникален. Введите заново: ");
                }
            }
            return id;
        }

        private bool isIdUnique(int id) {
            foreach (Factory schedule in this.schedules) {
                if (schedule.Id == id) {
                    return false;
                }
            }
            return true;
        }

        private void Delete() {
            findByChange("Введите ИД для удаления: ", "Удалено!", true);
        }

        private void Update() {
            findByChange("Введите ИД для редактирования: ", "Изменено!", false);
        }

        private void findByChange(string headMessage, string okMessage, bool isRemove) {
            this.Read(this.schedules);
            Console.Write(headMessage);
            int id = Program.EnterIntNumber();
            foreach (Factory schedule in this.schedules) {
                if (schedule.Id == id) {
                    chooseAction(isRemove, schedule, id);
                    Console.WriteLine(okMessage);
                    return;
                }
            }
            Console.WriteLine("Запись не найдена!");
        }

        private void chooseAction(bool isRemove, Factory schedule, int id) {
            if (isRemove) {
                this.schedules.Remove(schedule);
            } else {
                this.UpdateField(id);
            }
        }

        private void UpdateField(int id) {
            int fieldId = this.ChooseField();
            Console.Write("Введите новое значение: ");
            switch (fieldId) {
                case 1:
                    this.GetById(id).Id = this.EnterUniqueId(); break;
                case 2:
                    this.GetById(id).Year = Program.EnterDoubleNumber(); break;
                case 3:
                    this.GetById(id).Faculty = Program.EnterString(); break;
                case 4:
                    this.GetById(id).Form = Program.EnterString(); break;
                case 5:
                    this.GetById(id).Specialty = Program.EnterString(); break;
                case 6:
                    this.GetById(id).Subject = Program.EnterString(); break;
                case 7:
                    this.GetById(id).Hour = Program.EnterDoubleNumber(); break;
                default:
                    Console.WriteLine("Некорректный номер поля!"); break;
            }
        }

        private Factory GetById(int id) {
            foreach (Factory schedule in this.schedules) {
                if (schedule.Id == id) {
                    return schedule;
                }
            }
            return null;
        }
        
        private int ChooseField() {
            Console.WriteLine("Выберите поле: ");
            Console.WriteLine("1) ИД");
            Console.WriteLine("2) Год поступления");
            Console.WriteLine("3) Факультет");
            Console.WriteLine("4) Форма обучения");
            Console.WriteLine("5) Специальность");
            Console.WriteLine("6) Дисциплина");
            Console.WriteLine("7) Часы");
            return Program.EnterIntNumber();
        }
        
        private void Sort() {
            Console.WriteLine("Поиск по элементам!");
            int fieldId = ChooseField();
            switch (fieldId) {
                case 1:
                    this.schedules = this.schedules.OrderBy(o => o.Id).ToList(); break;
                case 2:
                    this.schedules = this.schedules.OrderBy(o => o.Year).ToList(); break;
                case 3:
                    this.schedules = this.schedules.OrderBy(o => o.Faculty).ToList(); break;
                case 4:
                    this.schedules = this.schedules.OrderBy(o => o.Form).ToList(); break;
                case 5:
                    this.schedules = this.schedules.OrderBy(o => o.Specialty).ToList(); break;
                case 6:
                    this.schedules = this.schedules.OrderBy(o => o.Subject).ToList(); break;
                case 7:
                    this.schedules = this.schedules.OrderBy(o => o.Hour).ToList(); break;
                default:
                    Console.WriteLine("Некорректный номер поля!"); break;
            }
            this.Read(this.schedules);
        }

        private void Find() {
            Console.WriteLine("Поиск по элементам!");
            int fieldId = ChooseField();
            Console.Write("Введите данные для поиска: ");
            string value = Program.EnterString();
            switch (fieldId) {
                case 1:
                    this.Read(this.schedules.Where(o => value == o.Id.ToString()).ToList()); break;
                case 2:
                    this.Read(this.schedules.Where(o => value == o.Year.ToString()).ToList()); break;
                case 3:
                    this.Read(this.schedules.Where(o => value == o.Faculty).ToList()); break;
                case 4:
                    this.Read(this.schedules.Where(o => value == o.Form).ToList()); break;
                case 5:
                    this.Read(this.schedules.Where(o => value == o.Specialty).ToList()); break;
                case 6:
                    this.Read(this.schedules.Where(o => value == o.Subject).ToList()); break;
                case 7:
                    this.Read(this.schedules.Where(o => value == o.Hour.ToString()).ToList()); break;
                default:
                    Console.WriteLine("Некорректный номер поля!"); break;
            }
        }

        private static int EnterIntNumber() {
            string value = Console.ReadLine();
            int result;
            if (!Int32.TryParse(value, out result)) {
                Console.Write("Это не целое число, введите заново: ");
                return Program.EnterIntNumber();
            } else {
                return result;
            }
        }

        private static string EnterString() {
            return Console.ReadLine();
        }

        private static double EnterDoubleNumber() {
            string value = Console.ReadLine();
            double result;
            if (!Double.TryParse(value, out result)) {
                Console.Write("Это не число, введите заново: ");
                return Program.EnterIntNumber();
            } else {
                return result;
            }
        }

    }
}
