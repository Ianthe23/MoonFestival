using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.Json;
using FestivalMuzica.Networking.Protocol;

namespace FestivalMuzica.Networking.Connection
{
    public class NetworkConnection : IDisposable
    {
        private readonly TcpClient _client;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private bool _isConnected;

        public NetworkConnection(string host, int port)
        {
            _client = new TcpClient();
            _client.Connect(host, port);
            var stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };
            _isConnected = true;
        }

        public async Task<NetworkMessage> SendMessageAsync(NetworkMessage message)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to server");

            try
            {
                var json = JsonSerializer.Serialize(message);
                await _writer.WriteLineAsync(json);

                var responseJson = await _reader.ReadLineAsync();
                if (string.IsNullOrEmpty(responseJson))
                    throw new IOException("Connection closed by server");

                return JsonSerializer.Deserialize<NetworkMessage>(responseJson);
            }
            catch (Exception ex)
            {
                _isConnected = false;
                throw new Exception("Error sending message", ex);
            }
        }

        public bool IsConnected => _isConnected && _client.Connected;

        public void Dispose()
        {
            _isConnected = false;
            _reader?.Dispose();
            _writer?.Dispose();
            _client?.Dispose();
        }
    }
} 