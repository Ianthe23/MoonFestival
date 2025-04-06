using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FestivalMuzica.Networking.Connection;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Common.Service;
using FestivalMuzica.Common.Exceptions;

namespace FestivalMuzica.Client.Service
{
    public class ServiceFestival : IService<long>
    {
        private readonly NetworkClient _networkClient;

        public ServiceFestival()
        {
            _networkClient = new NetworkClient(); // Uses default localhost:55555
        }

        public Employee? loginEmployee(string username, string password)
        {
            try
            {
                var result = Task.Run(async () => 
                    await _networkClient.LoginAsync(username, password)).Result;
                return result.Success ? result.Employee : null;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Login failed", ex);
            }
        }

        public Employee? registerEmployee(string username, string password)
        {
            try
            {
                var result = Task.Run(async () => 
                    await _networkClient.RegisterEmployeeAsync(username, password)).Result;
                return result.Success ? result.Employee : null;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Registration failed", ex);
            }
        }

        public List<FestivalMuzica.Common.Models.Client> getClients()
        {
            try
            {
                return Task.Run(async () => 
                    await _networkClient.GetClientsAsync()).Result;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to get clients", ex);
            }
        }

        public void addClient(FestivalMuzica.Common.Models.Client client)
        {
            try
            {
                Task.Run(async () => 
                    await _networkClient.AddClientAsync(client)).Wait();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to add client", ex);
            }
        }

        public List<FestivalMuzica.Common.Models.Show> getShows()
        {
            try
            {
                return Task.Run(async () => 
                    await _networkClient.GetShowsAsync()).Result;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to get shows", ex);
            }
        }

        public void updateShow(FestivalMuzica.Common.Models.Show show)
        {
            // This method is required by the interface but we don't want to implement it
            throw new NotImplementedException("updateShow is not implemented");
        }

        public List<FestivalMuzica.Common.Models.Ticket> getTickets()
        {
            try
            {
                return Task.Run(async () => 
                    await _networkClient.GetTicketsAsync()).Result;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to get tickets", ex);
            }
        }

        public void addTicket(FestivalMuzica.Common.Models.Ticket ticket)
        {
            try
            {
                Task.Run(async () => 
                    await _networkClient.AddTicketAsync(ticket)).Wait();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to add ticket", ex);
            }
        }

        public void sellTicket(FestivalMuzica.Common.Models.Show show, string clientName, string numberOfSeats)
        {
            try
            {
                Task.Run(async () => 
                    await _networkClient.SellTicketAsync(show, clientName, numberOfSeats)).Wait();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to sell ticket", ex);
            }
        }

        public List<FestivalMuzica.Common.Models.Show> getShowsByArtistAndTime(string artist, string time)
        {
            try
            {
                return Task.Run(async () => 
                    await _networkClient.GetShowsByArtistAndTimeAsync(artist, time)).Result;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to get shows by artist and time", ex);
            }
        }
    }
}
