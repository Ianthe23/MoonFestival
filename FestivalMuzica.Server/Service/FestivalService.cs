using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FestivalMuzica.Common.Models;
using FestivalMuzica.Common.Service;
using FestivalMuzica.Common.Exceptions;
using Microsoft.Extensions.Logging;
using FestivalMuzica.Server.Repository;
using Microsoft.AspNetCore.SignalR;
using FestivalMuzica.Server.Hubs;

namespace FestivalMuzica.Server.Service
{
    public class FestivalService : IFestivalService, IService<long>
    {
        private readonly ILogger<FestivalService> _logger;
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IClientRepo _clientRepo;
        private readonly IShowRepo _showRepo;
        private readonly ITicketRepo _ticketRepo;
        private IHubContext<FestivalHub> _hubContext;
        private readonly HubContextAccessor<FestivalHub> _hubContextAccessor;

        public FestivalService(
            ILogger<FestivalService> logger,
            IEmployeeRepo employeeRepo,
            IClientRepo clientRepo,
            IShowRepo showRepo,
            ITicketRepo ticketRepo,
            HubContextAccessor<FestivalHub> hubContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _employeeRepo = employeeRepo ?? throw new ArgumentNullException(nameof(employeeRepo));
            _clientRepo = clientRepo ?? throw new ArgumentNullException(nameof(clientRepo));
            _showRepo = showRepo ?? throw new ArgumentNullException(nameof(showRepo));
            _ticketRepo = ticketRepo ?? throw new ArgumentNullException(nameof(ticketRepo));
            _hubContextAccessor = hubContextAccessor ?? throw new ArgumentNullException(nameof(hubContextAccessor));
            
            // If the HubContext is already set, use it
            if (_hubContextAccessor.HubContext != null)
            {
                _hubContext = _hubContextAccessor.HubContext;
            }
            
            // Subscribe to changes in the hub context
            _hubContextAccessor.HubContextChanged += (sender, hubContext) => 
            {
                _hubContext = hubContext;
                _logger.LogInformation("HubContext is now available in FestivalService");
            };
        }

        public Employee loginEmployee(string username, string password)
        {
            try
            {
                _logger.LogInformation($"Attempting login for user: {username}");
                var employee = _employeeRepo.FindByUsernameAndPassword(username, password);
                if (employee == null)
                {
                    _logger.LogWarning($"Login failed for user: {username}");
                    return null;
                }
                _logger.LogInformation($"Login successful for user: {username}");
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for user: {username}");
                throw new ServiceException("Login failed", ex);
            }
        }

        public Employee registerEmployee(string username, string password)
        {
            try
            {
                _logger.LogInformation($"Attempting to register user: {username}");
                // Check if user already exists
                var existingEmployee = _employeeRepo.FindByUsernameAndPassword(username, password);
                if (existingEmployee != null)
                {
                    _logger.LogWarning($"Registration failed - user already exists: {username}");
                    return null;
                }

                // Create new employee
                var employee = new Employee
                {
                    Username = username,
                    Password = password
                };

                // Save employee
                _employeeRepo.Save(employee);

                _logger.LogInformation($"Registration successful for user: {username}");
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during registration for user: {username}");
                throw new ServiceException("Registration failed", ex);
            }
        }

        public List<FestivalMuzica.Common.Models.Client> getClients()
        {
            try
            {
                _logger.LogInformation("Getting all clients");
                var clients = _clientRepo.FindAll();
                _logger.LogInformation($"Retrieved {clients.Count()} clients");
                return clients.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clients");
                throw new ServiceException("Failed to get clients", ex);
            }
        }

        public void addClient(FestivalMuzica.Common.Models.Client client)
        {
            try
            {
                _logger.LogInformation($"Adding client: {client.Name}");
                _clientRepo.Save(client);
                _logger.LogInformation($"Client added successfully: {client.Name}");
                
                // Notify clients via SignalR
                if (_hubContext != null)
                {
                    _hubContext.Clients.All.SendAsync("ClientAdded", client).ConfigureAwait(false);
                    _logger.LogInformation($"Sent SignalR notification for new client: {client.Name}");
                }
                else
                {
                    _logger.LogWarning("SignalR HubContext not available, client notification skipped");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding client: {client.Name}");
                throw new ServiceException("Failed to add client", ex);
            }
        }

        public List<Show> getShows()
        {
            try
            {
                _logger.LogInformation("Getting all shows");
                var shows = _showRepo.FindAll();
                _logger.LogInformation($"Retrieved {shows.Count()} shows");
                return shows.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shows");
                throw new ServiceException("Failed to get shows", ex);
            }
        }

        public void updateShow(Show show)
        {
            try
            {
                _logger.LogInformation($"Updating show: {show.Name}");
                _showRepo.Update(show);
                _logger.LogInformation($"Show updated successfully: {show.Name}");
                
                // Notify clients via SignalR
                if (_hubContext != null)
                {
                    _logger.LogInformation($"Sending SignalR notification for show update: {show.Name}");
                    try
                    {
                        // Use multiple notifications to ensure all clients get the update
                        _hubContext.Clients.All.SendAsync("ShowUpdated", show)
                            .ContinueWith(task => {
                                if (task.IsCompletedSuccessfully)
                                    _logger.LogInformation($"ShowUpdated notification sent successfully");
                                else
                                    _logger.LogError(task.Exception, "Error sending ShowUpdated notification");
                            });
                        
                        // Send a second notification with a slight delay
                        System.Threading.Tasks.Task.Delay(100).ContinueWith(_ => {
                            _hubContext.Clients.All.SendAsync("ShowUpdated", show);
                            _logger.LogInformation("Sent second ShowUpdated notification");
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending SignalR notification: {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogWarning("SignalR HubContext not available, show update notification skipped");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating show: {show.Name}");
                throw new ServiceException("Failed to update show", ex);
            }
        }

        public List<Show> getShowsByArtistAndTime(string artist, string time)
        {
            try
            {
                _logger.LogInformation($"Getting shows for artist: {artist}, time: {time}");
                var shows = _showRepo.FindByArtistAndTime(artist, time);
                _logger.LogInformation($"Retrieved {shows.Count()} shows");
                return shows.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting shows for artist: {artist}, time: {time}");
                throw new ServiceException("Failed to get shows", ex);
            }
        }

        public List<Ticket> getTickets()
        {
            try
            {
                _logger.LogInformation("Getting all tickets");
                var tickets = _ticketRepo.FindAll();
                _logger.LogInformation($"Retrieved {tickets.Count()} tickets");
                return tickets.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tickets");
                throw new ServiceException("Failed to get tickets", ex);
            }
        }

        public void addTicket(Ticket ticket)
        {
            try
            {
                _logger.LogInformation($"Adding ticket for show: {ticket.ShowName}");
                _ticketRepo.Save(ticket);
                _logger.LogInformation($"Ticket added successfully for show: {ticket.ShowName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding ticket for show: {ticket.ShowName}");
                throw new ServiceException("Failed to add ticket", ex);
            }
        }

        public void sellTicket(Show show, string clientName, string numberOfSeats)
        {
            Client? client = null;
            try
            {
                _logger.LogInformation($"Selling ticket for show: {show.Name}, client: {clientName}, seats: {numberOfSeats}");

                // Find client
                var clientFound = _clientRepo.FindByName(clientName);
                if (clientFound == null)
                {
                    _logger.LogInformation($"Client not found, creating new client");
                    client = new Client
                    {
                        Name = clientName
                    };
                    _clientRepo.Save(client);
                    _logger.LogInformation($"Client created successfully: {clientName}");

                    client = _clientRepo.FindByName(clientName);
                    if (client == null)
                    {
                        _logger.LogError($"Client not found after saving: {clientName}");
                        throw new ServiceException($"Client not found after saving: {clientName}");
                    }
                    
                    // Notify about new client
                    if (_hubContext != null)
                    {
                        _hubContext.Clients.All.SendAsync("ClientAdded", client).ConfigureAwait(false);
                        _logger.LogInformation($"Sent SignalR notification for new client: {client.Name}");
                    }
                    else
                    {
                        _logger.LogWarning("SignalR HubContext not available, client notification skipped");
                    }
                }
                else
                {
                    client = clientFound;
                }

                // Check available seats
                if (show.AvailableSeats < int.Parse(numberOfSeats))
                {
                    _logger.LogWarning($"Not enough seats available for show: {show.Name}");
                    throw new ServiceException($"Not enough seats available for show: {show.Name}");
                }

                // Update show seats
                show.AvailableSeats -= int.Parse(numberOfSeats);
                show.SoldSeats += int.Parse(numberOfSeats);
                _showRepo.Update(show);
                
                // Notify about updated show
                if (_hubContext != null)
                {
                    _logger.LogInformation($"Sending SignalR notification for show update: {show.Name}");
                    try
                    {
                        // Use multiple notifications to ensure all clients get the update
                        _hubContext.Clients.All.SendAsync("ShowUpdated", show)
                            .ContinueWith(task => {
                                if (task.IsCompletedSuccessfully)
                                    _logger.LogInformation($"ShowUpdated notification sent successfully");
                                else
                                    _logger.LogError(task.Exception, "Error sending ShowUpdated notification");
                            });
                        
                        // Send a second notification with a slight delay
                        System.Threading.Tasks.Task.Delay(100).ContinueWith(_ => {
                            _hubContext.Clients.All.SendAsync("ShowUpdated", show);
                            _logger.LogInformation("Sent second ShowUpdated notification");
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending SignalR notification: {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogWarning("SignalR HubContext not available, show update notification skipped");
                }

                _logger.LogInformation($"Creating ticket for show: {show.Name}, client: {clientName}, client id: {client.Id}, seats: {numberOfSeats}");

                // Create ticket
                var ticket = new Ticket
                {
                    ShowId = show.Id,
                    ShowName = show.Name,
                    Client = client,
                    NumberOfSeats = int.Parse(numberOfSeats),
                    Price = 10
                };

                // Save ticket
                _ticketRepo.Save(ticket);
                
                // Notify about new ticket
                if (_hubContext != null)
                {
                    _logger.LogInformation($"Sending SignalR notification for new ticket: {ticket.ShowName}, Client: {ticket.Client.Name}");
                    try
                    {
                        // Send ticket sold notification
                        _hubContext.Clients.All.SendAsync("TicketSold", ticket)
                            .ContinueWith(task => {
                                if (task.IsCompletedSuccessfully)
                                    _logger.LogInformation($"TicketSold notification sent successfully");
                                else
                                    _logger.LogError(task.Exception, "Error sending TicketSold notification");
                            });
                        
                        // Also send show updated notification
                        _hubContext.Clients.All.SendAsync("ShowUpdated", show)
                            .ContinueWith(task => {
                                if (task.IsCompletedSuccessfully)
                                    _logger.LogInformation($"ShowUpdated notification sent after ticket sale");
                                else
                                    _logger.LogError(task.Exception, "Error sending ShowUpdated notification after ticket sale");
                            });
                        
                        // Send a second set of notifications with a slight delay to ensure clients get them
                        System.Threading.Tasks.Task.Delay(200).ContinueWith(_ => {
                            _hubContext.Clients.All.SendAsync("TicketSold", ticket);
                            _hubContext.Clients.All.SendAsync("ShowUpdated", show);
                            _logger.LogInformation("Sent delayed notifications after ticket sale");
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending SignalR notifications for ticket: {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogWarning("SignalR HubContext not available, ticket notification skipped");
                }

                // Add this line in the sellTicket method right after the regular notification
                try
                {
                    // Also try to send to each client individually using the static helper
                    FestivalMuzica.Server.Hubs.FestivalHub.SendToAllConnections(_hubContext, "TicketSold", ticket).ConfigureAwait(false);
                    FestivalMuzica.Server.Hubs.FestivalHub.SendToAllConnections(_hubContext, "ShowUpdated", show).ConfigureAwait(false);
                    
                    _logger.LogInformation("Used static helper to send to all connections individually");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error using static notification helper");
                }

                _logger.LogInformation($"Ticket sold successfully for show: {show.Name}, client: {clientName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error selling ticket for show: {show.Name}, client: {clientName}");
                throw new ServiceException("Failed to sell ticket", ex);
            }
        }

        public async Task<Employee> LoginEmployeeAsync(string username, string password)
        {
            return await Task.Run(() => loginEmployee(username, password));
        }

        public async Task<Employee> RegisterEmployeeAsync(string username, string password)
        {
            return await Task.Run(() => registerEmployee(username, password));
        }

        public async Task<List<FestivalMuzica.Common.Models.Client>> GetClientsAsync()
        {
            return await Task.Run(() => getClients());
        }

        public async Task AddClientAsync(FestivalMuzica.Common.Models.Client client)
        {
            await Task.Run(() => addClient(client));
        }

        public async Task<List<Show>> GetShowsAsync()
        {
            return await Task.Run(() => getShows());
        }

        public async Task UpdateShowAsync(Show show)
        {
            await Task.Run(() => updateShow(show));
        }

        public async Task<List<Show>> GetShowsByArtistAndTimeAsync(string artist, string time)
        {
            return await Task.Run(() => getShowsByArtistAndTime(artist, time));
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            return await Task.Run(() => getTickets());
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            await Task.Run(() => addTicket(ticket));
        }

        public async Task SellTicketAsync(Show show, string clientName, string numberOfSeats)
        {
            await Task.Run(() => sellTicket(show, clientName, numberOfSeats));
        }
    }
} 