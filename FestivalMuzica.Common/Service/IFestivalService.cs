using System.Collections.Generic;
using System.Threading.Tasks;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.Service
{
    public interface IFestivalService
    {
        // Authentication
        Task<Employee> LoginEmployeeAsync(string username, string password);
        Task<Employee> RegisterEmployeeAsync(string username, string password);

        // Client operations
        Task<List<Client>> GetClientsAsync();
        Task AddClientAsync(Client client);

        // Show operations
        Task<List<Show>> GetShowsAsync();
        Task UpdateShowAsync(Show show);
        Task<List<Show>> GetShowsByArtistAndTimeAsync(string artist, string time);

        // Ticket operations
        Task<List<Ticket>> GetTicketsAsync();
        Task AddTicketAsync(Ticket ticket);
        Task SellTicketAsync(Show show, string clientName, string numberOfSeats);
    }
} 