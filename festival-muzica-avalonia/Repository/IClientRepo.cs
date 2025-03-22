using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Repository
{
    public interface IClientRepo : IRepository<long, Client>
    {
        Client? FindByName(string name);
    }
}
