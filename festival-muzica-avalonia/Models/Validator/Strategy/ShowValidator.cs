using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Models.Validator.Strategy
{
    public class ShowValidator : IValidator<Show>
    {
        public void Validate(Show entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
                throw new ValidationException("Show name cannot be empty");
            if (string.IsNullOrEmpty(entity.ArtistName))
                throw new ValidationException("Artist name cannot be empty");
            if (string.IsNullOrEmpty(entity.Location))
                throw new ValidationException("Location cannot be empty");
            if (entity.AvailableSeats <= 0)
                throw new ValidationException("Available seats must be greater than 0");
            if (entity.SoldSeats < 0)
                throw new ValidationException("Sold seats cannot be negative");
        }
    }
} 