using System.Collections.Generic;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.Service
{
    public interface IService<TId>
    {
        // Authentication
        Employee loginEmployee(string username, string password);
        Employee registerEmployee(string username, string password);

        // Client operations
        List<Client> getClients();
        void addClient(Client client);

        // Show operations
        List<Show> getShows();
        void updateShow(Show show);
        List<Show> getShowsByArtistAndTime(string artist, string time);

        // Ticket operations
        List<Ticket> getTickets();
        void addTicket(Ticket ticket);
        void sellTicket(Show show, string clientName, string numberOfSeats);
    }
} 