namespace festival_muzica_avalonia.Models.Validator.Strategy
{
    public interface IValidator<T>
    {
        void Validate(T entity);
    }
} 