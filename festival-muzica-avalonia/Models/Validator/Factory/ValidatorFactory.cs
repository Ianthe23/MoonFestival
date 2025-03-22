using festival_muzica_avalonia.Models.Validator.Strategy;
using System;

namespace festival_muzica_avalonia.Models.Validator.Factory
{
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
            return strategy switch
            {
                EValidatorStrategy.Client => (IValidator<T>)new ClientValidator(),
                EValidatorStrategy.Employee => (IValidator<T>)new EmployeeValidator(),
                EValidatorStrategy.Ticket => (IValidator<T>)new TicketValidator(),
                EValidatorStrategy.Show => (IValidator<T>)new ShowValidator(),
                _ => throw new ArgumentException("Invalid validator strategy")
            };
        }
    }
}