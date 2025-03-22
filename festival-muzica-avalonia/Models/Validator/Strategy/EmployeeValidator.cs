using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Models.Validator.Strategy
{
    public class EmployeeValidator : IValidator<Employee>
    {
        public void Validate(Employee entity)
        {
            if (string.IsNullOrEmpty(entity.Username))
                throw new ValidationException("Employee username cannot be empty");
            if (string.IsNullOrEmpty(entity.Password))
                throw new ValidationException("Employee password cannot be empty");
        }
    }
} 