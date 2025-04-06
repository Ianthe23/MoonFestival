using System;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Common.Service;
using FestivalMuzica.Server.Protocol;

namespace FestivalMuzica.Server.Server
{
    public class ClientHandler : IDisposable
    {
        private readonly TcpClient _client;
        private readonly string _clientId;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly IService<long> _service;
        private bool _isRunning;
        private Employee _currentEmployee;

        public ClientHandler(TcpClient client, string clientId, IService<long> service)
        {
            _client = client;
            _clientId = clientId;
            _service = service;
            var stream = client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public async Task StartAsync()
        {
            _isRunning = true;
            
            try
            {
                while (_isRunning && _client.Connected)
                {
                    var messageJson = await _reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(messageJson)) break;

                    var message = JsonSerializer.Deserialize<NetworkMessage>(messageJson);
                    await ProcessMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing messages for client {_clientId}: {ex.Message}");
            }
        }

        private async Task ProcessMessageAsync(NetworkMessage message)
        {
            try
            {
                NetworkMessage response;
                
                // Check authentication for all operations except Login and Register
                if (message.Type != MessageType.Login && message.Type != MessageType.Register && _currentEmployee == null)
                {
                    response = new NetworkMessage(MessageType.Error, "Not authenticated");
                    await SendMessageAsync(response);
                    return;
                }

                switch (message.Type)
                {
                    case MessageType.Login:
                        var loginRequest = message.GetData<LoginRequest>();
                        var employee = _service.loginEmployee(loginRequest.Username, loginRequest.Password);
                        if (employee != null)
                        {
                            _currentEmployee = employee;
                            response = new NetworkMessage(MessageType.Success, employee);
                        }
                        else
                        {
                            response = new NetworkMessage(MessageType.Error, "Invalid credentials");
                        }
                        break;
                    
                    case MessageType.Register:
                        var registerRequest = message.GetData<LoginRequest>();
                        var newEmployee = _service.registerEmployee(registerRequest.Username, registerRequest.Password);
                        if (newEmployee != null)
                        {
                            _currentEmployee = newEmployee;
                            response = new NetworkMessage(MessageType.Success, newEmployee);
                        }
                        else
                        {
                            response = new NetworkMessage(MessageType.Error, "Registration failed");
                        }
                        break;
                    
                    case MessageType.Logout:
                        _currentEmployee = null;
                        response = new NetworkMessage(MessageType.Success, "Logged out successfully");
                        break;

                    case MessageType.GetShows:
                        var shows = _service.getShows();
                        response = new NetworkMessage(MessageType.Success, shows);
                        break;
                    
                    case MessageType.GetShowsByArtistAndTime:
                        var searchRequest = message.GetData<ShowSearchRequest>();
                        var filteredShows = _service.getShowsByArtistAndTime(searchRequest.Artist, searchRequest.Time);
                        response = new NetworkMessage(MessageType.Success, filteredShows);
                        break;
                    
                    case MessageType.UpdateShow:
                        var show = message.GetData<Show>();
                        try
                        {
                            _service.updateShow(show);
                            response = new NetworkMessage(MessageType.Success, "Show updated successfully");
                        }
                        catch (Exception ex)
                        {
                            response = new NetworkMessage(MessageType.Error, ex.Message);
                        }
                        break;
                    
                    case MessageType.GetTickets:
                        var tickets = _service.getTickets();
                        response = new NetworkMessage(MessageType.Success, tickets);
                        break;
                    
                    case MessageType.AddTicket:
                        var ticket = message.GetData<Ticket>();
                        try
                        {
                            _service.addTicket(ticket);
                            response = new NetworkMessage(MessageType.Success, "Ticket added successfully");
                        }
                        catch (Exception ex)
                        {
                            response = new NetworkMessage(MessageType.Error, ex.Message);
                        }
                        break;

                    case MessageType.GetClients:
                        var clients = _service.getClients();
                        response = new NetworkMessage(MessageType.Success, clients);
                        break;
                    
                    case MessageType.AddClient:
                        var client = message.GetData<Client>();
                        try
                        {
                            _service.addClient(client);
                            response = new NetworkMessage(MessageType.Success, "Client added successfully");
                        }
                        catch (Exception ex)
                        {
                            response = new NetworkMessage(MessageType.Error, ex.Message);
                        }
                        break;
                    
                    case MessageType.SellTicket:
                        var ticketRequest = message.GetData<TicketRequest>();
                        try
                        {
                            _service.sellTicket(ticketRequest.Show, ticketRequest.ClientName, ticketRequest.NumberOfSeats);
                            response = new NetworkMessage(MessageType.Success, "Ticket purchased successfully");
                        }
                        catch (Exception ex)
                        {
                            response = new NetworkMessage(MessageType.Error, ex.Message);
                        }
                        break;
                    
                    default:
                        response = new NetworkMessage(MessageType.Error, "Unknown command");
                        break;
                }

                await SendMessageAsync(response);
            }
            catch (Exception ex)
            {
                await SendMessageAsync(new NetworkMessage(MessageType.Error, ex.Message));
            }
        }

        private async Task SendMessageAsync(NetworkMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            await _writer.WriteLineAsync(json);
        }

        public void Dispose()
        {
            _isRunning = false;
            _reader?.Dispose();
            _writer?.Dispose();
            _client?.Dispose();
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        // Parameterless constructor for JSON deserialization
        public LoginRequest()
        {
        }
    }

    public class TicketRequest
    {
        public Show Show { get; set; }
        public string ClientName { get; set; }
        public string NumberOfSeats { get; set; }
        public int Price { get; set; }

        // Parameterless constructor for JSON deserialization
        public TicketRequest()
        {
        }
    }

    public class ShowSearchRequest
    {
        public string Artist { get; set; }
        public string Time { get; set; }

        // Parameterless constructor for JSON deserialization
        public ShowSearchRequest()
        {
        }
    }
} 