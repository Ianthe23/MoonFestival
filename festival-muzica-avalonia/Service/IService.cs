using System.Collections.Generic;
using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Service
{
    public interface IService<ID>
    {
        Employee? loginEmployee(string username, string password);
        Employee? registerEmployee(string username, string password);

        List<Client> getClients();
        void addClient(Client client);
        List<Show> getShows();

        void updateShow(Show show);
        List<Ticket> getTickets();

        void addTicket(Ticket ticket);
        void sellTicket(Show show, string clientName, string numberOfSeats);

        List<Show> getShowsByArtistAndTime(string artist, string time);
    }
}