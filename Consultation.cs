using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;

namespace Hospital
{
    public class Consultation : IComparable<Consultation>
    {
        public DateTime StartOfConsultation { get; set; }
        public DateTime EndOfConsultation { get; set; }
        public Patient Patient { get; set; }

        public Consultation() { }

        public Consultation(DateTime startOfConsultation, DateTime endOfConsultation, Patient patient)
        {
            this.StartOfConsultation = startOfConsultation;
            this.EndOfConsultation = endOfConsultation;
            this.Patient = patient;
        }

        public Consultation(DateTime startOfConsultation, Patient patient)
        {
            this.StartOfConsultation = startOfConsultation;
            this.Patient = patient;
        }

        public int CompareTo(Consultation other)
        {
            return this.StartOfConsultation.CompareTo(other.StartOfConsultation);
        }

        public override string ToString()
        {
            return Patient.Name + " " + Patient.Surname + " " + StartOfConsultation.ToString().Substring(0, 16) + " - " + EndOfConsultation.ToString().Substring(0, 16);
        }
    }
}
