using System;

namespace festival_muzica_avalonia.Models.Validator.Strategy
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
} 