using System;
using System.Text.Json;

namespace FestivalMuzica.Networking.Protocol
{
    public enum MessageType
    {
        // Authentication
        Login,
        Register,
        Logout,

        // Show operations
        GetShows,
        GetShowsByArtistAndTime,
        UpdateShow,

        // Ticket operations
        GetTickets,
        AddTicket,
        SellTicket,

        // Client operations
        GetClients,
        AddClient,

        // Response types
        Error,
        Success
    }

    [Serializable]
    public class NetworkMessage
    {
        public MessageType Type { get; set; }
        public string Data { get; set; }

        // Parameterless constructor for JSON deserialization
        public NetworkMessage()
        {
        }

        public NetworkMessage(MessageType type, object data)
        {
            Type = type;
            Data = JsonSerializer.Serialize(data);
        }

        public T GetData<T>()
        {
            return JsonSerializer.Deserialize<T>(Data);
        }
    }
} 