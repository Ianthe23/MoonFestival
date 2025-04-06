using System.Collections.Generic;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Client.Service
{
    public interface IService<ID>
    {
        Employee? loginEmployee(string username, string password);
        Employee? registerEmployee(string username, string password);

        List<FestivalMuzica.Common.Models.Client> getClients();
        void addClient(FestivalMuzica.Common.Models.Client client);
        List<Show> getShows();

        void updateShow(Show show);
        List<Ticket> getTickets();

        void addTicket(Ticket ticket);
        void sellTicket(Show show, string clientName, string numberOfSeats);

        List<Show> getShowsByArtistAndTime(string artist, string time);
    }
}