using System;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.DTOs
{
    [Serializable]
    public class AddClientDTO
    {
        public Client Client { get; set; }
    }

    [Serializable]
    public class ClientResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Client Client { get; set; }
    }
} 