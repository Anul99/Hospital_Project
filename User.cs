using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital
{
    public interface IUser : IComparable<IUser>
    {
        string Name { get; set; }
        string Surname { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        string Possition { get; set; }
    }
    public class User : IComparable<User>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Possition { get; set; }

        public int CompareTo(User other)
        {
            return this.Name.CompareTo(other.Name);
        }
    }
}
