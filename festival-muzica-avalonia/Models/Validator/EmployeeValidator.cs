using System.Linq.Expressions;

namespace festival_muzica_avalonia.Models
{
    public class EmployeeValidator : IValidator<Employee>
    {
        public void validate(Employee entity)
        {
            string errors = "";
            if (entity == null)
            {
                errors += "Employee is null\n";
            }
            else if (entity.Username == null || entity.Username.Length == 0)
            {
                errors += "Employee username is invalid\n";
            }
            else if (entity.Password == null || entity.Password.Length == 0)
            {
                errors += "Employee password is invalid\n";
            }

            if (errors.Length > 0)
            {
                throw new ValidationException(errors);
            }


        }
    }
}
