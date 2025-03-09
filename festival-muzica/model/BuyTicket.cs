public class BuyTicket : Entity<long>
{
    public Client client { get; set; }
    public Ticket ticket { get; set; }

    public BuyTicket(Client client, Ticket ticket)
    {
        this.client = client;
        this.ticket = ticket;
    }

    public BuyTicket(long id, Client client, Ticket ticket)
    {
        Id = id;
        this.client = client;
        this.ticket = ticket;
    }

    public override string ToString()
    {
        return "BuyTicket{" +
               "id=" + Id +
               ", client=" + client +
               ", ticket=" + ticket +
               '}';
    }

    public override bool Equals(object? obj)
    {
        if (this == obj) return true;
        if (obj == null || GetType() != obj.GetType()) return false;

        BuyTicket buyTicket = (BuyTicket)obj;

        return Id == buyTicket.Id &&
               (client == buyTicket.client || (client != null && client.Equals(buyTicket.client))) &&
               (ticket == buyTicket.ticket || (ticket != null && ticket.Equals(buyTicket.ticket)));
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, client, ticket);
    }
}