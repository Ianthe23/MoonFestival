using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Server.Repository
{
    public interface IEmployeeRepo : IRepository<long, Employee>
    {
        Employee? FindByUsernameAndPassword(string username, string password);
    }
}