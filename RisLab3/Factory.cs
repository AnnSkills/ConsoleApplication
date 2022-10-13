using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace RisLab3 {
    [DataContract]
    class Factory
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public double Year { get; set; }

        [DataMember]
        public string Faculty { get; set; }

        [DataMember]
        public string Form { get; set; }

        [DataMember]
        public string Specialty { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public double Hour { get; set; }

        public Factory(int id, double year, string faculty, string form, string specialty, string subject, double hour)
        {
            this.Id = id;
            this.Year = year;
            this.Faculty = faculty;
            this.Form = form;
            this.Specialty = specialty;
            this.Subject = subject;
            this.Hour = hour;
        }

        public Factory() {
            this.Id = 0;
            this.Year = 0;
            this.Faculty = "";
            this.Form = "";
            this.Specialty = "";
            this.Subject = "";
            this.Hour = 0;
        }
    }
}
