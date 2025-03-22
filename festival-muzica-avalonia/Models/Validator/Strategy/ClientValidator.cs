using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Models.Validator.Strategy
{
    public class ClientValidator : IValidator<Client>
    {
        public void Validate(Client entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
                throw new ValidationException("Client name cannot be empty");
        }
    }
} 