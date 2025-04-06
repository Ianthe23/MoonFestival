using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using FestivalMuzica.Common.Models;
using System.Collections.Generic;
using System.Threading;

namespace FestivalMuzica.Client.Service
{
    public class SignalRService : IDisposable
    {
        private HubConnection _hubConnection;
        private readonly string _hubUrl;
        private volatile bool _isConnected;
        private readonly Timer _reconnectTimer;
        private readonly object _reconnectLock = new object();
        private readonly string _clientId;

        // Events that other components can subscribe to
        public event Action<Show> OnShowUpdated;
        public event Action<Ticket> OnTicketSold;
        public event Action<FestivalMuzica.Common.Models.Client> OnClientAdded;
        public event Action<string> OnConnected;
        public event Action OnDisconnected;
        public event Action<Exception> OnError;

        public SignalRService(string serverUrl = "http://localhost", int port = 5000, string clientId = null)
        {
            _clientId = clientId ?? Guid.NewGuid().ToString("N");
            _hubUrl = $"{serverUrl}:{port}/festivalhub";
            Console.WriteLine($"SignalRService created for hub: {_hubUrl}, client ID: {_clientId}");
            
            // Create a timer that checks connection every 5 seconds
            _reconnectTimer = new Timer(
                CheckConnection, 
                null, 
                TimeSpan.FromSeconds(5), 
                TimeSpan.FromSeconds(5));
                
            InitializeConnection(_clientId);
        }
        
        private void CheckConnection(object state)
        {
            lock (_reconnectLock)
            {
                if (_hubConnection != null && 
                    _hubConnection.State == HubConnectionState.Disconnected)
                {
                    Console.WriteLine("Connection check detected disconnected state. Attempting reconnect...");
                    try
                    {
                        // Connect asynchronously
                        _ = StartConnectionAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during reconnect attempt: {ex.Message}");
                    }
                }
            }
        }

        private void InitializeConnection(string clientId = null)
        {
            try
            {
                Console.WriteLine($"Initializing SignalR connection to {_hubUrl} with client ID: {clientId ?? "none"}");
                
                // Use more resilient connection with proper logging
                var builder = new HubConnectionBuilder()
                    .WithUrl(_hubUrl)
                    .WithAutomaticReconnect(new[] { 
                        TimeSpan.FromSeconds(0),  // First retry immediately
                        TimeSpan.FromSeconds(2),  // Second retry after 2 seconds
                        TimeSpan.FromSeconds(5),  // Then 5 seconds
                        TimeSpan.FromSeconds(10),  // Then 10 seconds
                        TimeSpan.FromSeconds(15)   // Then 15 seconds
                    });
                    
                // Create and build the connection
                _hubConnection = builder.Build();

                // Set up event handlers with better error handling
                _hubConnection.On<Show>("ShowUpdated", show =>
                {
                    try {
                        Console.WriteLine($"[RECEIVED] SignalR event: Show updated - {show.Name} (Available: {show.AvailableSeats}, Sold: {show.SoldSeats})");
                        OnShowUpdated?.Invoke(show);
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error handling ShowUpdated event: {ex.Message}");
                    }
                });

                _hubConnection.On<Ticket>("TicketSold", ticket =>
                {
                    try {
                        Console.WriteLine($"[RECEIVED] SignalR event: Ticket sold for show: {ticket.ShowName}");
                        OnTicketSold?.Invoke(ticket);
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error handling TicketSold event: {ex.Message}");
                    }
                });

                _hubConnection.On<FestivalMuzica.Common.Models.Client>("ClientAdded", client =>
                {
                    try {
                        Console.WriteLine($"[RECEIVED] SignalR event: Client added: {client.Name}");
                        OnClientAdded?.Invoke(client);
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error handling ClientAdded event: {ex.Message}");
                    }
                });

                _hubConnection.On<string>("Connected", connectionId =>
                {
                    try {
                        Console.WriteLine($"[CONNECTED] Connected to hub with ID: {connectionId}");
                        _isConnected = true;
                        OnConnected?.Invoke(connectionId);
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error handling Connected event: {ex.Message}");
                    }
                });

                _hubConnection.Closed += error =>
                {
                    try {
                        Console.WriteLine($"[DISCONNECTED] Connection closed. Error: {error?.Message}");
                        _isConnected = false;
                        OnDisconnected?.Invoke();
                        
                        // Try to reconnect outside the event handler to avoid deadlocks
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(1000); // Wait 1 second before reconnecting
                            try
                            {
                                Console.WriteLine("[RECONNECTING] Attempting to reconnect after disconnection");
                                await StartConnectionAsync();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error reconnecting: {ex.Message}");
                            }
                        });
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error in closed event handler: {ex.Message}");
                    }
                    
                    return Task.CompletedTask;
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SignalR connection: {ex.Message}");
                OnError?.Invoke(ex);
            }
        }

        public async Task StartConnectionAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                Console.WriteLine("SignalR connection is already established");
                return;
            }

            try
            {
                Console.WriteLine($"Starting SignalR connection to {_hubUrl}...");
                await _hubConnection.StartAsync();
                Console.WriteLine($"SignalR connection established with ID: {_hubConnection.ConnectionId}");
                _isConnected = true;
                OnConnected?.Invoke(_hubConnection.ConnectionId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting SignalR connection: {ex.Message}");
                _isConnected = false;
                OnError?.Invoke(ex);
                throw;
            }
        }

        public async Task RestartConnectionAsync()
        {
            try
            {
                Console.WriteLine("Restarting SignalR connection...");
                
                // Stop the connection if it's active
                if (_hubConnection.State != HubConnectionState.Disconnected)
                {
                    await _hubConnection.StopAsync();
                    Console.WriteLine("SignalR connection stopped for restart");
                }
                
                // Reinitialize the connection
                InitializeConnection(_clientId);
                
                // Start the connection again
                await StartConnectionAsync();
                Console.WriteLine("SignalR connection restarted successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restarting SignalR connection: {ex.Message}");
                OnError?.Invoke(ex);
                throw;
            }
        }

        public bool IsConnected => _isConnected;
        public bool IsAuthenticated => true; // This value is not used but needed for the interface

        public void Dispose()
        {
            _reconnectTimer?.Dispose();
            _hubConnection?.DisposeAsync().GetAwaiter().GetResult();
        }
    }
} 