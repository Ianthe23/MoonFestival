namespace FestivalMuzica.Common.Models.Validator
{
    public interface IValidator<T>
    {
        void Validate(T entity);
    }
} 