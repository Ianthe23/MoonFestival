public class ValidatorFactory : IValidatorFactory
{
    private static ValidatorFactory? instance = null;

    private ValidatorFactory() { }

    public static ValidatorFactory getInstance()
    {
        if (instance == null)
        {
            instance = new ValidatorFactory();
        }
        return instance;
    }

    public IValidator<T> CreateValidator<T>(EValidatorStrategy strategy)
    {
        switch (strategy)
        {
            case EValidatorStrategy.Client:
                return (IValidator<T>)new ClientValidator();
            case EValidatorStrategy.Employee:
                return (IValidator<T>)new EmployeeValidator();
            case EValidatorStrategy.Ticket:
                return (IValidator<T>)new TicketValidator();
            case EValidatorStrategy.Show:
                return (IValidator<T>)new ShowValidator();
            default:
                throw new ValidationException("Invalid validator strategy");
        }
    }

}