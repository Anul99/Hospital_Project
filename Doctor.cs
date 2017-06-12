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
    public class Doctor : User
    {
        public string Speciality { get; set; }
        public List<WorkingTimes> WorkingTimes { get; set; }
        public List<Consultation> Calendar { get; set; }
        public string Telephone { get; set; }
        public int CostOfConsultation { get; set; }
        public List<Consultation> ListOfRequests { get; set; }
        public Doctor(string name, string surname, string login, string password, string speciality, List<WorkingTimes> workingTimes, string telephone, int costOfConsultion)
        {
            this.Name = name;
            this.Surname = surname;
            this.Login = login;
            this.Password = password;
            this.Possition = "Doctor";
            this.Speciality = speciality;
            this.WorkingTimes = workingTimes;
            this.Calendar = new List<Consultation>();
            this.Telephone = telephone;
            this.CostOfConsultation = costOfConsultion;
            this.ListOfRequests = new List<Consultation>();
        }

        public void AddNoteToPatientHistory(Patient patient, string note)
        {
            foreach (Consultation c in this.Calendar)
            {
                if (patient == c.Patient && c.StartOfConsultation < DateTime.Now)
                {
                    patient.History += note + "\n\nDoctor: " + this.Name + " " + this.Surname + " (" + this.Login + ")\n\n";
                    return;
                }
            }
            MessageBox.Show("You can't add note to the history of this patient.");
        }

        public void ServeAPatient(Consultation request)
        {
            if (request.StartOfConsultation > DateTime.Now)
            {
                this.Calendar.Add(request);
                request.Patient.Messages.Add(new Message("Your request for consultation was confirmed. \nYour consultation: " + request.StartOfConsultation.ToString().Substring(0, 16) + " - " + request.EndOfConsultation.TimeOfDay.ToString().Substring(0, 5) + "\n                   Doctor:" + this.Name + " " + this.Surname + " (" + this.Login + ")", DateTime.Now));
                this.ListOfRequests.Remove(request);
            }
            else
            {
                MessageBox.Show("You can't confirm request whith past date.");
            }

        }

        public void RefuseARequest(Consultation request)
        {
            request.Patient.Messages.Add(new Message("Your request for consultation on " + request.StartOfConsultation.ToString().Substring(0, 16) + " was refused.", DateTime.Now));
            ListOfRequests.Remove(request);

        }

        public void OpenListOfRequests()
        {
            for (int i = ListOfRequests.Count - 1; i >= 0; i--)
            {
                if (ListOfRequests[i].StartOfConsultation < DateTime.Now)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    ListOfRequests.RemoveAt(i);
                }
                Console.Write(ListOfRequests.Count - i + " ");
                PrintRequest(ListOfRequests[i]);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void SeeMyCalendar()
        {
            Calendar.Sort();
            for (int i = 0; i < Calendar.Count; i++)
            {
                if (Calendar[i].EndOfConsultation < DateTime.Now)
                {
                    Calendar.RemoveAt(i);
                    i--;
                }
                else
                {
                    Console.WriteLine(Calendar[i]);
                }
            }
        }

        public override string ToString()
        {
            string res = this.Possition + "/ " + this.Speciality + "\n";
            res += this.Name + " " + this.Surname + "\n" + "Login: " + this.Login + "\n" + "Tel:" + this.Telephone + "\n";
            foreach (WorkingTimes t in this.WorkingTimes)
            {
                res += t + "\n";
            }
            return res;
        }

        public void ChangePassword(string newPassword)
        {
            this.Password = newPassword;
        }

        public void PrintRequest(Consultation request)
        {
            Console.WriteLine(request.Patient.Name + " " + request.Patient.Surname + " " + request.StartOfConsultation.ToString().Substring(0, 16));
        }

    }

    public struct WorkingTimes
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartingTime { get; set; }
        public TimeSpan EndingTime { get; set; }

        public WorkingTimes(DayOfWeek dayOfWeek, string startingTime, string endingTime)
        {
            this.DayOfWeek = dayOfWeek;
            string[] startingT = startingTime.Split(':');
            string[] endingT = endingTime.Split(':');
            this.StartingTime = new TimeSpan(int.Parse(startingT[0].Trim()), int.Parse(startingT[1].Trim()), 0);
            this.EndingTime = new TimeSpan(int.Parse(endingT[0].Trim()), int.Parse(endingT[1].Trim()), 0);
        }

        public override string ToString()
        {
            return (this.DayOfWeek + " " + this.StartingTime + "-" + this.EndingTime);
        }

    }
}
