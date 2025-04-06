using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FestivalMuzica.Common.Service;
using FestivalMuzica.Server.Protocol;

namespace FestivalMuzica.Server.Server
{
    public class TcpServer
    {
        private TcpListener _listener;
        private readonly ConcurrentDictionary<string, ClientHandler> _clients;
        private readonly IService<long> _service;
        private bool _isRunning;
        private readonly int _port;

        public TcpServer(IService<long> service, int port = 55555)
        {
            _service = service;
            _port = port;
            _clients = new ConcurrentDictionary<string, ClientHandler>();
        }

        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _isRunning = true;

            try
            {
                _listener.Start();
                Console.WriteLine($"Server started on port {_port}");

                while (_isRunning)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Server error: {ex.Message}");
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var clientId = Guid.NewGuid().ToString();
            var handler = new ClientHandler(client, clientId, _service);
            
            try
            {
                _clients.TryAdd(clientId, handler);
                Console.WriteLine($"Client connected: {clientId}");
                
                await handler.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {clientId}: {ex.Message}");
            }
            finally
            {
                _clients.TryRemove(clientId, out _);
                handler.Dispose();
                Console.WriteLine($"Client disconnected: {clientId}");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            
            foreach (var client in _clients.Values)
            {
                client.Dispose();
            }
            _clients.Clear();
        }
    }
} 