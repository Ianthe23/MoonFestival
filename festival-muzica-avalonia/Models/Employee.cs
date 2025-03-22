using System;

namespace festival_muzica_avalonia.Models
{
    public class Employee : Entity<long>
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public Employee(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public Employee(long id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return "Employee{" +
                   "id=" + Id +
                   ", username='" + Username + '\'' +
                   ", password='" + Password + '\'' +
                   '}';
        }

        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Employee employee = (Employee)obj;
            return Id == employee.Id &&
                   Username.Equals(employee.Username) &&
                   Password.Equals(employee.Password);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Username, Password);
        }
    }
}