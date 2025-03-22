using System.Collections.Generic;
using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Repository
{
    public interface ITicketRepo : IRepository<long, Ticket>
    {
        IEnumerable<Ticket> FindByShow(Show show);
        IEnumerable<Ticket> FindByClient(Client client);
    }
}