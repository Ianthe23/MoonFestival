public class TicketValidator : IValidator<Ticket>
{
    public void validate(Ticket entity)
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
        else if (entity.client == null)
        {
            errors += "Client is invalid\n";
        }

        if (errors.Length > 0)
        {
            throw new ValidationException(errors);
        }
    }
}