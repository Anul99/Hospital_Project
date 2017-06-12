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
    public class Message
    {
        private bool read = false;

        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool Read { get { return read; } set { read = value; } }

        public Message(string text, DateTime time)
        {
            this.Text = text;
            this.Time = time;
            //this.Read = 0;
        }

        public override string ToString()
        {
            return this.Time + "\n" + this.Text + "\n";
        }
    }


    public class Patient : User
    {
        public string History { get; set; }
        public List<Message> Messages { get; set; }
        public int CountOfUnreadMessages
        {
            get
            {
                int count = 0;
                foreach (Message m in this.Messages)
                {
                    if (!m.Read)
                    {
                        count++;
                    }
                }
                return count;
            }
        }


        public Patient(string name, string surname, string login, string password)
        {
            this.Name = name;
            this.Surname = surname;
            this.Login = login;
            this.Password = password;
            this.Possition = "Patient";
            this.Messages = new List<Message>();
        }

        public void RequestForConsultation(Doctor doctor, DateTime startOfConsultation)
        {
            foreach (Consultation c in doctor.Calendar)
            {
                if (startOfConsultation >= c.StartOfConsultation && startOfConsultation < c.EndOfConsultation)
                {
                    MessageBox.Show("You can't have a consultation at this time.");
                }
            }
            doctor.ListOfRequests.Add(new Consultation(startOfConsultation, this));
        }

        public void SeeMyHistory()
        {
            Console.WriteLine(this.History);
        }

        public void SeeMyMessages()
        {
            int i = this.Messages.Count - 1;
            Console.Write("Messeges ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(this.CountOfUnreadMessages);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (i = this.Messages.Count - 1; i > this.Messages.Count - this.CountOfUnreadMessages - 1; i--)
            {
                Console.WriteLine(this.Messages[i]);
                this.Messages[i].Read = true;
            }
            Console.ForegroundColor = ConsoleColor.White;
            for (; i >= 0; i--)
            {
                Console.WriteLine(this.Messages[i]);
            }

        }

        public override string ToString()
        {
            return this.Name + " " + this.Surname + "\nLogin: " + this.Login + "\nHistory: " + this.History;
        }

        public void ChangePassword(string newPassword)
        {
            this.Password = newPassword;
        }
    }
}
