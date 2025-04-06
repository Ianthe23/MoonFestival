using System.Collections.Generic;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Server.Repository
{
    public interface ITicketRepo : IRepository<long, Ticket>
    {
        IEnumerable<Ticket> FindByShow(Show show);
        IEnumerable<Ticket> FindByClient(FestivalMuzica.Common.Models.Client client);
        IEnumerable<Ticket> FindByShowAndClient(Show show, FestivalMuzica.Common.Models.Client client);
    }
}