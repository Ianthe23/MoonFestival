using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Server.Repository
{
    public interface IClientRepo : IRepository<long, FestivalMuzica.Common.Models.Client>
    {
        FestivalMuzica.Common.Models.Client? FindByName(string name);
    }
}
