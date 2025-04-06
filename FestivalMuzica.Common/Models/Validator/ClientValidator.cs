using FestivalMuzica.Common.Models;
using FestivalMuzica.Common.Exceptions;

namespace FestivalMuzica.Common.Models.Validator
{
    public class ClientValidator : IValidator<Client>
    {
        public void Validate(Client entity)
        {
            if (entity == null)
            {
                throw new ValidationException("Client is null");
            }
            if (entity.Name == null || entity.Name.Length == 0)
            {
                throw new ValidationException("Client name is invalid");
            }
        }
    }
} 