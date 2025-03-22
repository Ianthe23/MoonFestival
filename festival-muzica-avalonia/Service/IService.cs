using System.Collections.Generic;
using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Service
{
    public interface IService<ID>
    {
        Employee? loginEmployee(string username, string password);
        Employee? registerEmployee(string username, string password);
        List<Show> getShows();
    }
}