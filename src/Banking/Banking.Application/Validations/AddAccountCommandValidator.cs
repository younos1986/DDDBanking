using Banking.Application.Commands;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Banking.Application.Validations
{
    public class AddAccountCommandValidator: AbstractValidator<AddAccountCommand>
    {
        public AddAccountCommandValidator(ILogger<AddAccountCommand> logger)
        {
            RuleFor(command => command.CustomerId).NotEmpty();

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
        
    }
}