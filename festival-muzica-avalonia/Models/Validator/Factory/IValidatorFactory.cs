using festival_muzica_avalonia.Models.Validator.Strategy;

namespace festival_muzica_avalonia.Models.Validator.Factory
{
    public interface IValidatorFactory
    {
        IValidator<T> CreateValidator<T>(EValidatorStrategy strategy);
    }
}