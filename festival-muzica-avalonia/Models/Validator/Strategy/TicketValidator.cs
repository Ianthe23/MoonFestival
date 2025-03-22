using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Models.Validator.Strategy
{
    public class TicketValidator : IValidator<Ticket>
    {
        public void Validate(Ticket entity)
        {
            if (entity.client == null)
                throw new ValidationException("Ticket must have a client");
            if (entity.ShowId <= 0)
                throw new ValidationException("Show ID must be greater than 0");
            if (entity.NumberOfSeats <= 0)
                throw new ValidationException("Number of seats must be greater than 0");
            if (entity.Price <= 0)
                throw new ValidationException("Price must be greater than 0");
        }
    }
} 