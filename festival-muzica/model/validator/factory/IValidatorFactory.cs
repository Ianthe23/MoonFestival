public interface IValidatorFactory
{
    IValidator<T> CreateValidator<T>(EValidatorStrategy strategy);
}