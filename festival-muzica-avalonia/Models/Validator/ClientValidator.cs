namespace festival_muzica_avalonia.Models
{
    public class ClientValidator : IValidator<Client>
    {
        public void validate(Client entity)
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
