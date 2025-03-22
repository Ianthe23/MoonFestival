using System;

public class ServiceException : Exception
    {
        // Default constructor
        public ServiceException()
        {
        }

        // Constructor that takes a message
        public ServiceException(string message)
            : base(message)
        {
        }

        // Constructor that takes a message and a cause (inner exception)
        public ServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor that takes a cause (inner exception)
        public ServiceException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        // Constructor that takes a message, cause, and the enableSuppression and writableStackTrace flags.
        public ServiceException(string message, Exception innerException, bool enableSuppression, bool writableStackTrace)
            : base(message, innerException)
        {
            // enableSuppression and writableStackTrace are handled by base class.
            // If needed, they can be manually adjusted in this constructor as well.
        }
    }