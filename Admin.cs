using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;

namespace Hospital
{
    class Admin : User
    {
        public Admin(string name, string surname, string login, string password)
        {
            this.Name = name;
            this.Surname = surname;
            this.Login = login;
            this.Password = password;
            this.Possition = "Admin";
        }

        public void AddDoctor(string name, string surname, string login, string password, string speciality, List<WorkingTimes> workingTimes, string telephone, int costOfConsultation)
        {
            Doctor newDoctor = new Doctor(name, surname, login, password, speciality, workingTimes, telephone, costOfConsultation);
            Hospital.AllDoctors.Add(newDoctor);
        }

        public void Report()
        {
            int money = 0;
            foreach (Doctor d in Hospital.AllDoctors)
            {
                int countOfConsultations = 0;
                foreach (Consultation c in d.Calendar)
                {
                    if (c.StartOfConsultation < DateTime.Now)
                    {
                        countOfConsultations++;
                    }
                }
                money += countOfConsultations * d.CostOfConsultation;
                Console.WriteLine("{0}: {1} {2} ({3})", d.Possition, d.Name, d.Surname, d.Login);
                Console.WriteLine("\tCount of consultations : {0}  Money : {1}", countOfConsultations, countOfConsultations * d.CostOfConsultation);
            }
            Console.WriteLine("--------------------------------");
            Console.WriteLine("Total money : {0}", money);
        }

        public void DeleteDoctor(Doctor doctor)
        {
            Hospital.AllDoctors.Remove(doctor);
        }

        public override string ToString()
        {
            return this.Possition + ": " + this.Name + " " + this.Surname + "\nLogin: " + this.Login;
        }
    }
}
