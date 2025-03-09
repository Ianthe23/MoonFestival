public class Employee : Entity<long>
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public Employee(string name, string username, string password)
    {
        Name = name;
        Username = username;
        Password = password;
    }

    public Employee(long id, string name, string username, string password)
    {
        Id = id;
        Name = name;
        Username = username;
        Password = password;
    }

    public override string ToString()
    {
        return "Employee{" +
               "id=" + Id +
               ", name='" + Name + '\'' +
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
               Name.Equals(employee.Name) &&
               Username.Equals(employee.Username) &&
               Password.Equals(employee.Password);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Username, Password);
    }
}