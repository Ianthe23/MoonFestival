using System;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.DTOs
{
    [Serializable]
    public class TicketPurchaseDTO
    {
        public Show Show { get; set; }
        public string ClientName { get; set; }
        public string NumberOfSeats { get; set; }
    }

    [Serializable]
    public class TicketResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Ticket Ticket { get; set; }
    }

    [Serializable]
    public class AddTicketDTO
    {
        public Ticket Ticket { get; set; }
    }
} 