using System;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.DTOs
{
    [Serializable]
    public class ShowSearchDTO
    {
        public string Artist { get; set; }
        public string Time { get; set; }
    }

    [Serializable]
    public class ShowUpdateDTO
    {
        public Show Show { get; set; }
    }

    [Serializable]
    public class ShowResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Show Show { get; set; }
    }
} 