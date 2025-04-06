using System.Collections.Generic;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.Repository
{
    public interface ITicketRepo : IRepository<long, Ticket>
    {
        IEnumerable<Ticket> FindByShow(Show show);
        IEnumerable<Ticket> FindByClient(Client client);
        IEnumerable<Ticket> FindByShowAndClient(Show show, Client client);
    }
} 