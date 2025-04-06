using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Networking.DTOs;
using FestivalMuzica.Networking.Protocol;

namespace FestivalMuzica.Networking.Connection
{
    public class NetworkClient : IDisposable
    {
        private readonly NetworkConnection _connection;
        private Employee _currentEmployee;

        public NetworkClient(string host = "localhost", int port = 55555)
        {
            _connection = new NetworkConnection(host, port);
        }

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var request = new NetworkMessage(MessageType.Login, new LoginRequest 
            { 
                Username = username, 
                Password = password 
            });

            var response = await _connection.SendMessageAsync(request);
            if (response.Type == MessageType.Success)
            {
                _currentEmployee = response.GetData<Employee>();
                return new LoginResponse 
                { 
                    Success = true, 
                    Employee = _currentEmployee 
                };
            }

            return new LoginResponse 
            { 
                Success = false, 
                Message = response.GetData<string>() 
            };
        }

        public async Task<LoginResponse> RegisterEmployeeAsync(string username, string password)
        {
            var request = new NetworkMessage(MessageType.Register, new LoginRequest 
            { 
                Username = username, 
                Password = password 
            });

            var response = await _connection.SendMessageAsync(request);
            if (response.Type == MessageType.Success)
            {
                _currentEmployee = response.GetData<Employee>();
                return new LoginResponse 
                { 
                    Success = true, 
                    Employee = _currentEmployee 
                };
            }

            return new LoginResponse 
            { 
                Success = false, 
                Message = response.GetData<string>() 
            };
        }

        public async Task LogoutAsync()
        {
            var request = new NetworkMessage(MessageType.Logout, null);
            await _connection.SendMessageAsync(request);
            _currentEmployee = null;
        }

        public async Task<List<Show>> GetShowsAsync()
        {
            var request = new NetworkMessage(MessageType.GetShows, null);
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type == MessageType.Success)
                return response.GetData<List<Show>>();
            
            throw new Exception(response.GetData<string>());
        }

        public async Task<List<Show>> GetShowsByArtistAndTimeAsync(string artist, string time)
        {
            var request = new NetworkMessage(MessageType.GetShowsByArtistAndTime, 
                new ShowSearchRequest { Artist = artist, Time = time });
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type == MessageType.Success)
                return response.GetData<List<Show>>();
            
            throw new Exception(response.GetData<string>());
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            var request = new NetworkMessage(MessageType.GetTickets, null);
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type == MessageType.Success)
                return response.GetData<List<Ticket>>();
            
            throw new Exception(response.GetData<string>());
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            var request = new NetworkMessage(MessageType.GetClients, null);
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type == MessageType.Success)
                return response.GetData<List<Client>>();
            
            throw new Exception(response.GetData<string>());
        }

        public async Task AddClientAsync(Client client)
        {
            var request = new NetworkMessage(MessageType.AddClient, client);
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type != MessageType.Success)
                throw new Exception(response.GetData<string>());
        }

        public async Task UpdateShowAsync(Show show)
        {
            var request = new NetworkMessage(MessageType.UpdateShow, show);
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type != MessageType.Success)
                throw new Exception(response.GetData<string>());
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            var request = new NetworkMessage(MessageType.AddTicket, ticket);
            var response = await _connection.SendMessageAsync(request);
            
            if (response.Type != MessageType.Success)
                throw new Exception(response.GetData<string>());
        }

        public async Task SellTicketAsync(Show show, string clientName, string numberOfSeats)
        {
            var request = new NetworkMessage(MessageType.SellTicket, 
                new TicketRequest
                {
                    Show = show,
                    ClientName = clientName,
                    NumberOfSeats = numberOfSeats,
                    Price = 10
                });
            
            var response = await _connection.SendMessageAsync(request);
            if (response.Type != MessageType.Success)
                throw new Exception(response.GetData<string>());
        }

        public bool IsConnected => _connection.IsConnected;
        public bool IsAuthenticated => _currentEmployee != null;

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
} 