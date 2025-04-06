using System;

namespace FestivalMuzica.Common.Exceptions
{
    public class ValidationException : Exception
    {
        // Default constructor
        public ValidationException()
        {
        }

        // Constructor that takes a message
        public ValidationException(string message)
            : base(message)
        {
        }

        // Constructor that takes a message and a cause (inner exception)
        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor that takes a cause (inner exception)
        public ValidationException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        // Constructor that takes a message, cause, and the enableSuppression and writableStackTrace flags.
        public ValidationException(string message, Exception innerException, bool enableSuppression, bool writableStackTrace)
            : base(message, innerException)
        {
            // enableSuppression and writableStackTrace are not directly needed because base class already handles them.
            // But you can manipulate them if you need to.
        }
    }
} 