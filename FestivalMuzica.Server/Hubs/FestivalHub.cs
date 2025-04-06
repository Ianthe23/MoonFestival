using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using FestivalMuzica.Common.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;

namespace FestivalMuzica.Server.Hubs
{
    public class FestivalHub : Hub
    {
        private readonly ILogger<FestivalHub> _logger;
        private static readonly HashSet<string> _connectedClients = new HashSet<string>();

        public FestivalHub(ILogger<FestivalHub> logger)
        {
            _logger = logger;
        }

        public async Task NotifyShowUpdated(Show show)
        {
            try
            {
                _logger.LogInformation($"Broadcasting ShowUpdated to {_connectedClients.Count} clients. Show: {show.Name}, Remaining seats: {show.AvailableSeats}");
                await Clients.All.SendAsync("ShowUpdated", show);
                
                // Also broadcast to specific groups if you have them
                await Clients.Group("shows").SendAsync("ShowUpdated", show);
                
                _logger.LogInformation("ShowUpdated broadcast completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting ShowUpdated event");
            }
        }

        public async Task NotifyTicketSold(Ticket ticket)
        {
            try
            {
                _logger.LogInformation($"Broadcasting TicketSold to {_connectedClients.Count} clients. Show: {ticket.ShowName}, Client: {ticket.Client.Name}");
                
                // Send to all connected clients
                await Clients.All.SendAsync("TicketSold", ticket);
                
                // Also broadcast the associated show update to ensure UI consistency
                var show = new Show { Id = ticket.ShowId, Name = ticket.ShowName };
                await Clients.All.SendAsync("ShowUpdated", show);
                
                _logger.LogInformation("TicketSold broadcast completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting TicketSold event");
            }
        }

        public async Task NotifyClientAdded(Client client)
        {
            _logger.LogInformation($"Broadcasting ClientAdded to {_connectedClients.Count} clients. Client: {client.Name}");
            await Clients.All.SendAsync("ClientAdded", client);
            _logger.LogInformation("ClientAdded broadcast completed");
        }

        public override async Task OnConnectedAsync()
        {
            _connectedClients.Add(Context.ConnectionId);
            _logger.LogInformation($"Client connected: {Context.ConnectionId}. Total connected clients: {_connectedClients.Count}");
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _connectedClients.Remove(Context.ConnectionId);
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}. Remaining clients: {_connectedClients.Count}. Reason: {exception?.Message ?? "Normal disconnect"}");
            await base.OnDisconnectedAsync(exception);
        }

        // Add a broadcast method that can be called from the service
        public async Task BroadcastShowUpdate(Show show)
        {
            try
            {
                _logger.LogInformation($"[EXPLICIT BROADCAST] Broadcasting show update to all clients: {show.Name}");
                
                // Send to All clients - this is the standard way
                await Clients.All.SendAsync("ShowUpdated", show);
                
                // Log all connection IDs this is going to
                _logger.LogInformation($"Broadcasting to {_connectedClients.Count} clients: {string.Join(", ", _connectedClients)}");
                
                // Also try sending to each client individually in case "All" has issues
                foreach (var connectionId in _connectedClients)
                {
                    try
                    {
                        _logger.LogInformation($"Sending direct show update to client: {connectionId}");
                        await Clients.Client(connectionId).SendAsync("ShowUpdated", show);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending to specific client {connectionId}");
                    }
                }
                
                _logger.LogInformation("[EXPLICIT BROADCAST] Show update broadcast completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting show update");
            }
        }

        public async Task BroadcastTicketSold(Ticket ticket)
        {
            try
            {
                _logger.LogInformation($"[EXPLICIT BROADCAST] Broadcasting ticket sold to all clients: {ticket.ShowName}");
                
                // Broadcast to all clients
                await Clients.All.SendAsync("TicketSold", ticket);
                
                // Try to broadcast individually as well
                foreach (var connectionId in _connectedClients)
                {
                    try
                    {
                        _logger.LogInformation($"Sending direct ticket sold to client: {connectionId}");
                        await Clients.Client(connectionId).SendAsync("TicketSold", ticket);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending to specific client {connectionId}");
                    }
                }
                
                // Also force a show update for UI consistency
                var show = new Show { 
                    Id = ticket.ShowId, 
                    Name = ticket.ShowName
                };
                await BroadcastShowUpdate(show);
                
                _logger.LogInformation("[EXPLICIT BROADCAST] Ticket sold broadcast completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting ticket sold");
            }
        }

        // Add a static method to get all connection IDs
        public static HashSet<string> GetAllConnectionIds()
        {
            return new HashSet<string>(_connectedClients);
        }

        // Add a method that can be called from outside to send to individual clients
        public static async Task SendToAllConnections(IHubContext<FestivalHub> hubContext, string methodName, object arg)
        {
            // First try the regular approach
            await hubContext.Clients.All.SendAsync(methodName, arg);
            
            // Then try sending to each client individually
            foreach (var connectionId in _connectedClients)
            {
                try
                {
                    await hubContext.Clients.Client(connectionId).SendAsync(methodName, arg);
                }
                catch
                {
                    // Ignore errors sending to individual clients
                }
            }
        }
    }
}
