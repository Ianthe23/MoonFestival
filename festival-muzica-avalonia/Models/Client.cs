using System;

public class Client : Entity<long>
{
    public string Name { get; set; }

    public Client(string name)
    {
        Name = name;
    }
    public Client(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return "Client{" +
               "id=" + Id +
               ", name='" + Name + '\'' +
               '}';
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;
        Client client = (Client)obj;
        return Id == client.Id &&
               Name.Equals(client.Name);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }
}