using FestivalMuzica.Common.Models;
using FestivalMuzica.Common.Exceptions;

namespace FestivalMuzica.Common.Models.Validator
{
    public class TicketValidator : IValidator<Ticket>
    {
        public void Validate(Ticket entity)
        {
            string errors = "";
            if (entity == null)
            {
                errors += "Ticket is null\n";
            }
            else if (entity.Id < 0)
            {
                errors += "Ticket id is invalid\n";
            }
            else if (entity.ShowId < 0)
            {
                errors += "Show id is invalid\n";
            }
            else if (entity.ShowName == null)
            {
                errors += "Show name is invalid\n";
            }
            else if (entity.Client == null)
            {
                errors += "Client is invalid\n";
            }
            else if (entity.NumberOfSeats < 0)
            {
                errors += "Number of seats is invalid\n";
            }
            else if (entity.Price < 0)
            {
                errors += "Price is invalid\n";
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
} 