using System;
using FestivalMuzica.Common.Models;

namespace FestivalMuzica.Common.DTOs
{
    [Serializable]
    public class LoginDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Employee Employee { get; set; }
    }

    [Serializable]
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
} 