using System;

public class Ticket : Entity<long>
{
    public long ShowId { get; set; }
    public string ShowName { get; set; }
    
    public Client client { get; set; }
    public int NumberOfSeats { get; set; }
    public int Price { get; set; }

    public Ticket(long showId, string showName, Client client, int numberOfSeats, int price)
    {
        ShowId = showId;
        ShowName = showName;
        this.client = client;
        NumberOfSeats = numberOfSeats;
        Price = price;
    }

    public Ticket(long id, long showId, string showName, Client client, int numberOfSeats, int price)
    {
        Id = id;
        ShowId = showId;
        ShowName = showName;
        this.client = client;
        NumberOfSeats = numberOfSeats;
        Price = price;
    }

    public override string ToString()
    {
        return "Ticket{" +
               "id=" + Id +
               ", showId=" + ShowId +
               ", showName=" + ShowName +
               ", client=" + client +
               ", numberOfSeats=" + NumberOfSeats +
               ", price=" + Price +
               '}';
    }

    public override bool Equals(object? obj)  // The '?' makes the obj parameter nullable.
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;

        Ticket ticket = (Ticket)obj;

        return Id == ticket.Id &&
               ShowId == ticket.ShowId &&
               ShowName == ticket.ShowName &&
               (client == ticket.client || (client != null && client.Equals(ticket.client))) &&
               NumberOfSeats == ticket.NumberOfSeats &&
               Price == ticket.Price;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ShowId, ShowName, client, NumberOfSeats, Price);
    }
}
