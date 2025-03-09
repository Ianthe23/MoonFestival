using System.Linq.Expressions;

public class EmployeeValidator : IValidator<Employee>
{
    public void validate(Employee entity)
    {
        string errors = "";
        if (entity == null)
        {
            errors += "Employee is null\n";
        }
        else if (entity.Name == null || entity.Name.Length == 0)
        {
            errors += "Employee name is invalid\n";
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