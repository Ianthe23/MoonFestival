namespace festival_muzica_avalonia.Models
{
    public class ShowValidator : IValidator<Show>
    {
        public void validate(Show entity)
        {
            string errors = "";
            if (entity == null)
            {
                errors += "Show is null\n";
            }
            else if (entity.Id < 0)
            {
                errors += "Show id is invalid\n";
            }
            else if (entity.Name == null || entity.Name.Length == 0)
            {
                errors += "Show name is invalid\n";
            }
            else if (entity.ArtistName == null || entity.ArtistName.Length == 0)
            {
                errors += "Show artists are invalid\n";
            }
            else if (entity.Location == null || entity.Location.Length == 0)
            {
                errors += "Show location is invalid\n";
            }
            else if (entity.Date.ToString() == null)
            {
                errors += "Show date is invalid\n";
            }
            else if (entity.AvailableSeats < 0)
            {
                errors += "Show available seats are invalid\n";
            }
            else if (entity.SoldSeats < 0)
            {
                errors += "Show sold seats are invalid\n";
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}
