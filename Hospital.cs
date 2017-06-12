using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.IO;
using System.Collections;

namespace Hospital
{
    public enum Command
    {
        signIn,
        signUp,
        signOut,
        search,
        exit,
        changePassword,

        //Patient's commands
        requestForConsultation,
        myHistory,
        myMesseges,

        //Doctor's commands
        addPatientHistory,
        openListOfRequests,
        confirm,
        refuse,
        myCalendar,

        //Admin's commands
        addDoctor,
        deleteDoctor,
        reports, //????????????

        nothing

    }

    public static class Hospital
    {
        private static string allPatientsPath = @"../../AllPatients.json";
        private static string allDoctorsPath = @"../../Doctors.json";
        private static string adminPath = @"../../Admin.json";


        private static List<Patient> allPatients = new List<Patient>();

        private static List<Doctor> allDoctors = new List<Doctor>();


        private static Admin admin = new Admin("Admin", "", "admin", HashSha256("1234"));

        public static List<Patient> AllPatients
        {
            get { return allPatients; }
            set { allPatients = value; }
        }

        public static List<Doctor> AllDoctors
        {
            get { return allDoctors; }
            set { allDoctors = value; }
        }



        public static Command DetectCommand(string str)
        {
            foreach (string c in Enum.GetNames(typeof(Command)))
            {
                if (str == c)
                {
                    Command command = (Command)Enum.Parse(typeof(Command), c);
                    return command;
                }
            }
            return Command.nothing;
        }

        public static User Search(string line)
        {
            List<User> foundUsers = new List<User>();
            List<Patient> foundPatients = new List<Patient>();
            List<Doctor> foundDoctors = new List<Doctor>();
            if (line == "")
            {
                AllPatients.Sort();
                foreach (Patient p in AllPatients)
                {
                    foundUsers.Add(p);
                }
                foreach (Doctor d in AllDoctors)
                {
                    foundUsers.Add(d);
                }
            }
            string[] s = line.Split();
            switch (s.Length)
            {
                case 1:
                    SearchByName(s[0], foundUsers);
                    SearchBySurname(s[0], foundUsers);
                    SearchByLogin(s[0], foundUsers);
                    break;
                case 2:
                    SearchByNameAndSurname(s[0], s[1], foundUsers);
                    SearchByNameAndSurname(s[1], s[0], foundUsers);
                    break;
                case 3:
                    SearchByNameSurnnameAndLogin(s[0], s[1], s[2], foundUsers);
                    SearchByNameSurnnameAndLogin(s[1], s[0], s[2], foundUsers);
                    SearchByNameSurnnameAndLogin(s[1], s[2], s[0], foundUsers);
                    SearchByNameSurnnameAndLogin(s[2], s[1], s[0], foundUsers);
                    break;
            }
            if (foundUsers.Count == 0)
            {
                Console.WriteLine("There is no user with this name.");
                return null;
            }
            else if (foundUsers.Count == 1)
            {
                return foundUsers[0];
            }
            else
            {
                foundUsers.Sort();
                for (int i = 0; i < foundUsers.Count; i++)
                {
                    Console.Write(i + 1 + ". ");
                    PrintUser(foundUsers[i]);
                }
                return Select(foundUsers);

            }
        }

        private static void SearchByName(string name, List<User> foundUsers)
        {
            foreach (Patient u in AllPatients)
            {
                if (u.Name.ToLower() == name.ToLower())
                    foundUsers.Add(u);
            }
            foreach (Doctor u in allDoctors)
            {
                if (u.Name.ToLower() == name.ToLower())
                    foundUsers.Add(u);
            }
        }

        private static void SearchBySurname(string surname, List<User> foundUsers)
        {
            foreach (Patient u in AllPatients)
            {
                if (u.Surname.ToLower() == surname.ToLower())
                    foundUsers.Add(u);
            }
            foreach (Doctor u in allDoctors)
            {
                if (u.Surname.ToLower() == surname.ToLower())
                    foundUsers.Add(u);
            }
        }

        private static void SearchByNameAndSurname(string name, string surname, List<User> foundUsers)
        {
            foreach (Patient u in AllPatients)
            {
                if (u.Name.ToLower() == name.ToLower() && u.Surname.ToLower() == surname.ToLower())
                    foundUsers.Add(u);
            }
            foreach (Doctor u in allDoctors)
            {
                if (u.Name.ToLower() == name.ToLower() && u.Surname.ToLower() == surname.ToLower())
                    foundUsers.Add(u);
            }
        }

        private static void SearchByLogin(string login, List<User> foundUsers)
        {
            foreach (Patient u in AllPatients)
            {
                if (u.Login == login)
                    foundUsers.Add(u);
            }
            foreach (Doctor u in allDoctors)
            {
                if (u.Login == login)
                    foundUsers.Add(u);
            }
        }

        private static void SearchByNameSurnnameAndLogin(string name, string surname, string login, List<User> foundUsers)
        {
            foreach (Patient u in AllPatients)
            {
                if (u.Name.ToLower() == name.ToLower() && u.Surname.ToLower() == surname.ToLower() && u.Login == login)
                    foundUsers.Add(u);
            }
            foreach (Doctor u in allDoctors)
            {
                if (u.Name.ToLower() == name.ToLower() && u.Surname.ToLower() == surname.ToLower() && u.Login == login)
                    foundUsers.Add(u);
            }
        }


        public static User Select(List<User> list)
        {
            try
            {
                int num = Convert.ToInt32(Console.ReadLine());
                if (num < 1 || num > list.Count)
                {
                    MessageBox.Show("Write correct number.");
                    return null;
                }
                else
                {
                    return list[num - 1];
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Write a number.");
                return null;
            }
        }

        public static Consultation Select(List<Consultation> list, string s)
        {
            try
            {
                int num = Convert.ToInt32(s);
                if (num < 1 || num > list.Count)
                {
                    MessageBox.Show("Incorrect number.");
                    return null;
                }
                else
                {
                    return list[list.Count - num];
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Write a number.");
                return null;
            }
        }

        public static void SignIn()
        {
            if (myUser == null)
            {
                Console.Write("Login: ");
                string login = Console.ReadLine();
                Console.Write("Password: ");
                string password = HashSha256(GetPasswordFromUser());
                Console.WriteLine();
                foreach (Patient p in AllPatients)
                {
                    if (login == p.Login && password == p.Password)
                    {
                        myUser = p;
                        Console.WriteLine();
                        PrintUser(myUser);
                        Console.Write("\nMesseges ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(p.Messages.Count);
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }
                foreach (Doctor d in allDoctors)
                {
                    if (login == d.Login && password == d.Password)
                    {
                        myUser = d;
                        Console.WriteLine();
                        PrintUser(myUser);
                        Console.Write("\nRequests ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        myDoctor = (Doctor)myUser;
                        Console.WriteLine(myDoctor.ListOfRequests.Count);
                        Console.ForegroundColor = ConsoleColor.White;
                        return;
                    }
                }

                if (login == admin.Login && password == admin.Password)
                {
                    myUser = admin;
                    Console.WriteLine();
                    PrintUser(myUser);
                    return;
                }
                MessageBox.Show("Incorrect login or password.");
            }
            else
            {
                MessageBox.Show("You are already loged in.");
            }
        }

        public static string[] SignUp()
        {
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Surame: ");
            string surname = Console.ReadLine();
            bool t = true;
            string login = "";
            do
            {
                Console.Write("Login: ");
                login = Console.ReadLine();
                foreach (Patient p in AllPatients)
                {
                    if (login == p.Login)
                    {
                        MessageBox.Show("This login already exists.");
                        t = false;
                    }
                }
                if (!t)
                {
                    foreach (Doctor d in allDoctors)
                    {
                        if (login == d.Login)
                        {
                            MessageBox.Show("This login already exists.");
                            t = false;
                        }
                    }
                }
            }
            while (!t);
            Console.Write("Password: ");
            string password = HashSha256(GetPasswordFromUser());
            Console.WriteLine();
            string[] res = { name, surname, login, password };
            return res;
        }

        public static void ChangePassword(User user)
        {
            Console.Write("Current password: ");
            string currentPassword = HashSha256(GetPasswordFromUser());
            if (user.Password != currentPassword)
            {
                MessageBox.Show("Wrong password.");
            }
            else
            {
                Console.Write("\nNew password: ");
                string newPassword = GetPasswordFromUser();
                Console.Write("\nRe-type new password: ");
                string retypedpassword = GetPasswordFromUser();
                if (newPassword != retypedpassword)
                {
                    MessageBox.Show("You must enter the same password twice in order to confirm it.");
                }
                else
                {
                    user.Password = HashSha256(newPassword);
                }
            }
        }

        public static void AddDoctor()
        {
            string[] userData = SignUp();
            Console.Write("Speciality: ");
            string speciality = Console.ReadLine();
            Console.Write("Working times: (hh:mm - hh:mm / doesn't work)\n");
            List<WorkingTimes> workingTimes = new List<WorkingTimes>();
            foreach (string d in Enum.GetNames(typeof(DayOfWeek)))
            {
                bool t = false;
                while (!t)
                {
                    Console.Write(d + " ");
                    string s = Console.ReadLine();
                    string[] times = s.Split('-');
                    try
                    {
                        workingTimes.Add(new WorkingTimes((DayOfWeek)Enum.Parse(typeof(DayOfWeek), d), times[0].Trim(), times[1].Trim()));
                        t = true;
                    }
                    catch
                    {
                        if (s != "doesn't work")
                            t = true;
                        else
                            MessageBox.Show("Write on format  hh:mm - hh:mm / doesn't work");

                    }
                }
            }

            Console.Write("Telephone: ");
            string tel = Console.ReadLine();
            bool f = false;
            int costOfConsultation = 0;
            do
            {
                Console.Write("Cost of consultation: ");
                try
                {
                    costOfConsultation = int.Parse(Console.ReadLine());
                    f = true;
                }
                catch (Exception)
                {
                    MessageBox.Show("Write cost of consultation (by numbers).");
                }
            }
            while (!f);

            Admin myAdmin = (Admin)myUser;
            myAdmin.AddDoctor(userData[0], userData[1], userData[2], userData[3], speciality, workingTimes, tel, costOfConsultation);
        }

        public static void SignOut()
        {
            if (myUser != null)
                myUser = null;
            else
                MessageBox.Show("You are not signed in.");
        }

        private static string GetPasswordFromUser()
        {
            string password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);
            return password;
        }

        public static void RequestForConsultation(string date)
        {
            if (selected == null)
            {
                Console.WriteLine("There is no selected doctor.");
            }
            else
            {
                try
                {
                    Doctor doctor = (Doctor)selected;
                    doctor.Calendar.Sort();
                    foreach (Consultation c in doctor.Calendar)
                    {
                        if (c.StartOfConsultation > DateTime.Now)
                        {
                            Console.WriteLine(c);
                        }
                    }
                    Console.WriteLine("Write date and time for consultation.");
                    date = Console.ReadLine();
                    try
                    {
                        string[] dateAndTime = date.Split('/', '.', ' ', ':');
                        DateTime startOfConsultation = new DateTime(int.Parse(dateAndTime[2]), int.Parse(dateAndTime[1]), int.Parse(dateAndTime[0]), int.Parse(dateAndTime[3]), int.Parse(dateAndTime[4]), 0);
                        Patient myPatient = (Patient)myUser;
                        myPatient.RequestForConsultation(doctor, startOfConsultation);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Please enter date in DD/MM/YYYY HH:MM or DD.MM.YYYY HH:MM format.");
                    }
                }
                catch { }
            }
        }

        public static void ConfirmRequest(Consultation request)
        {
            if (request != null)
            {
                Console.WriteLine("Write duration of consulteation(by minutes).");
                bool t = false;
                int duration = 0;
                while (!t)
                {
                    try
                    {
                        duration = Convert.ToInt32(Console.ReadLine());
                        t = true;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Write duration of consulteation by minutes.");
                    }
                }
                Consultation newConsultation = new Consultation(request.StartOfConsultation, request.StartOfConsultation.AddMinutes(duration), request.Patient);
                Doctor myDoctor = (Doctor)myUser;
                myDoctor.ServeAPatient(newConsultation);
                myDoctor.ListOfRequests.Remove(request);

            }
        }

        private static void PrintUser(User user)
        {
            Console.WriteLine("{0}: {1} {2} ({3})", user.Possition, user.Name, user.Surname, user.Login);
        }

        public static string HashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }





        private static Patient myPatient = null;
        private static Doctor myDoctor = null;
        private static Admin myAdmin = null;
        private static User myUser = null;
        private static User selected = null;




        public static void MyConsole()
        {
            AllPatients = JsonConvert.DeserializeObject<List<Patient>>(File.ReadAllText(allPatientsPath));
            allDoctors = JsonConvert.DeserializeObject<List<Doctor>>(File.ReadAllText(allDoctorsPath));
            admin = JsonConvert.DeserializeObject<Admin>(File.ReadAllText(adminPath));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("WELCOME TO OUR HOSPITAL :)\n");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Active commands: " + (Command)0);
            for (int i = 1; i <= (int)Command.exit; i++)
            {
                Console.WriteLine("                 " + (Command)i);
            }

            Command command = Command.nothing;
            while (command != Command.exit)
            {
                if (myUser != null)
                {
                    if (myUser.Possition == "Patient")
                    {
                        Console.WriteLine();
                        PrintUser(myUser);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Active commands: " + (Command)2);
                        for (int i = 3; i <= (int)Command.myMesseges; i++)
                        {
                            if ((selected == null || selected.Possition != "Doctor") && (Command)i == Command.requestForConsultation)
                                continue;
                            Console.WriteLine("                 " + (Command)i);
                        }
                    }
                    else if (myUser.Possition == "Doctor")
                    {
                        Console.WriteLine();
                        PrintUser(myUser);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Active commands: " + (Command)2);
                        for (int i = 3; i <= (int)Command.changePassword; i++)
                        {
                            Console.WriteLine("                 " + (Command)i);
                        }
                        for (int i = (int)Command.addPatientHistory; i <= (int)Command.myCalendar; i++)
                        {
                            if ((selected == null || selected.Possition != "Patient") && (Command)i == Command.addPatientHistory)
                                continue;
                            Console.WriteLine("                 " + (Command)i);
                        }
                    }
                    else if (myUser.Possition == "Admin")
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("Active commands: " + (Command)2);
                        for (int i = 3; i <= (int)Command.changePassword; i++)
                        {
                            Console.WriteLine("                 " + (Command)i);
                        }
                        for (int i = (int)Command.addDoctor; i <= (int)Command.reports; i++)
                        {
                            if ((selected == null || selected.Possition != "Doctor") && (Command)i == Command.deleteDoctor)
                                continue;
                            Console.WriteLine("                 " + (Command)i);
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        PrintUser(myUser);
                    }
                    if (selected != null)
                    {
                        Console.Write("Opened Page: ");
                        PrintUser(selected);
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("$ ");
                String line = Console.ReadLine();
                command = DetectCommand(line.Split()[0]);
                line = line.Replace(command.ToString(), "").Trim();
                switch (command)
                {
                    case Command.nothing:
                        MessageBox.Show("Incorrect command!");
                        break;
                    case Command.search:
                        selected = Search(line);
                        Console.WriteLine(selected);
                        break;
                    case Command.signIn:
                        if (myUser == null)
                            SignIn();
                        else
                            MessageBox.Show("You are already loged in.");
                        break;
                    case Command.signUp:
                        if (myUser == null)
                        {
                            string[] userData = SignUp();
                            myUser = new Patient(userData[0], userData[1], userData[2], userData[3]);
                            try
                            {
                                allPatients.Add((Patient)myUser);
                            }
                            catch { }
                        }
                        else
                            MessageBox.Show("You are already signed up.");
                        break;
                    case Command.signOut:
                        SignOut();
                        break;
                    case Command.changePassword:
                        if (myUser != null)
                        {
                            ChangePassword(myUser);
                        }
                        break;
                    case Command.requestForConsultation:
                        if (myUser != null && selected != null && selected.Possition == "Doctor")
                            RequestForConsultation(line.Replace(command.ToString(), "").Trim());
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.myHistory:
                        if (myUser != null && myUser.Possition == "Patient")
                        {
                            myPatient = (Patient)myUser;
                            myPatient.SeeMyHistory();
                        }
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.myMesseges:
                        if (myUser != null && myUser.Possition == "Patient")
                        {
                            myPatient = (Patient)myUser;
                            myPatient.SeeMyMessages();
                        }
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.addPatientHistory:
                        if (myUser != null && myUser.Possition == "Doctor" && selected.Possition == "Patient")
                        {
                            myDoctor = (Doctor)myUser;
                            if (selected != null && selected.Possition == "Patient")
                            {
                                myDoctor.AddNoteToPatientHistory((Patient)selected, line.Replace(command.ToString(), "").Trim());
                            }
                            else
                            {
                                MessageBox.Show("There is no patient selected.");
                            }
                        }
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.openListOfRequests:
                        if (myUser != null && myUser.Possition == "Doctor")
                            myDoctor.OpenListOfRequests();
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.confirm:
                        if (myUser != null && myUser.Possition == "Doctor")
                            ConfirmRequest(Select(myDoctor.ListOfRequests, line.Replace(command.ToString(), "").Trim()));
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.refuse:
                        if (myUser != null && myUser.Possition == "Doctor")
                        {
                            Consultation request = Select(myDoctor.ListOfRequests, line.Replace(command.ToString(), "").Trim());
                            if (request != null)
                            {
                                myDoctor.RefuseARequest(request);
                            }
                        }
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.myCalendar:
                        if (myUser != null && myUser.Possition == "Doctor")
                            myDoctor.SeeMyCalendar();
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.addDoctor:
                        if (myUser != null && myUser.Possition == "Admin")
                            AddDoctor();
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                    case Command.deleteDoctor:
                        if (myUser != null && myUser.Possition == "Admin")
                        {
                            if (selected != null && selected.Possition == "Doctor")
                            {
                                myAdmin = (Admin)myUser;
                                myAdmin.DeleteDoctor((Doctor)selected);
                            }
                            else
                                MessageBox.Show("There is no doctor selected.");
                        }
                        else
                            MessageBox.Show("Incorrect command.");
                        break;
                }
                File.WriteAllText(allPatientsPath, JsonConvert.SerializeObject(AllPatients));
                File.WriteAllText(allDoctorsPath, JsonConvert.SerializeObject(AllDoctors));
                File.WriteAllText(adminPath, JsonConvert.SerializeObject(admin));
            }
        }
    }
}
