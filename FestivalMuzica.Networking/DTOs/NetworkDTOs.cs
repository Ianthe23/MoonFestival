using System;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Networking.DTOs
{
    [Serializable]
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        // Parameterless constructor for JSON deserialization
        public LoginRequest()
        {
        }
    }

    [Serializable]
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Employee Employee { get; set; }

        // Parameterless constructor for JSON deserialization
        public LoginResponse()
        {
        }
    }

    [Serializable]
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

    [Serializable]
    public class ShowSearchRequest
    {
        public string Artist { get; set; }
        public string Time { get; set; }

        // Parameterless constructor for JSON deserialization
        public ShowSearchRequest()
        {
        }
    }

    [Serializable]
    public class NetworkResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        // Parameterless constructor for JSON deserialization
        public NetworkResponse()
        {
        }
    }
} 