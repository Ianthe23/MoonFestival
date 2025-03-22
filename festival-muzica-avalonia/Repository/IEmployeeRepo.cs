using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Repository
{
    public interface IEmployeeRepo : IRepository<long, Employee>
    {
        Employee? FindByUsernameAndPassword(string username, string password);
    }
}