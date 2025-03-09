public class Ticket : Entity<long>
{
    public long ShowId { get; set; }
    public Client client { get; set; }
    public int NumberOfSeats { get; set; }

    public Ticket(long showId, Client client, int numberOfSeats)
    {
        ShowId = showId;
        this.client = client;
        NumberOfSeats = numberOfSeats;
    }

    public Ticket(long id, long showId, Client client, int numberOfSeats)
    {
        Id = id;
        ShowId = showId;
        this.client = client;
        NumberOfSeats = numberOfSeats;
    }

    public override string ToString()
    {
        return "Ticket{" +
               "id=" + Id +
               ", showId=" + ShowId +
               ", client=" + client +
               ", numberOfSeats=" + NumberOfSeats +
               '}';
    }

    public override bool Equals(object? obj)  // The '?' makes the obj parameter nullable.
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;

        Ticket ticket = (Ticket)obj;

        return Id == ticket.Id &&
               ShowId == ticket.ShowId &&
               (client == ticket.client || (client != null && client.Equals(ticket.client))) &&
               NumberOfSeats == ticket.NumberOfSeats;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ShowId, client, NumberOfSeats);
    }
}
