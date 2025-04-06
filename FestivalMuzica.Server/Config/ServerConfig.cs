namespace FestivalMuzica.Server.Config
{
    public class ServerConfig
    {
        public int Port { get; set; } = 55555;
        public int SignalRPort { get; set; } = 5000;
        public string DbConnectionString { get; set; }
        public string LogPath { get; set; } = "logs/server.log";
    }
} 