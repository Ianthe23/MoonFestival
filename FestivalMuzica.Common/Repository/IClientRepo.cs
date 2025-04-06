using System.Collections.Generic;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.Repository
{
    public interface IClientRepo : IRepository<long, Client>
    {
        Client? FindByName(string name);
    }
} 