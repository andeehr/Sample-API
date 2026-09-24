using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sample.Common.Exceptions;
using Sample.Core.Services.Interfaces;

namespace Sample.Core.Services
{
    public class ValidatorService : IValidatorService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidatorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Validate<T>(T request)
        {
            var validator = _serviceProvider.GetService<IValidator<T>>() ?? throw new ArgumentException($"Validator not found: {typeof(T).Name}");

            var result = validator.Validate(request);

            if (!result.IsValid)
                throw new DomainException($"Request is invalid: {string.Join(". ", result.Errors.Select(e => e.ErrorMessage))}");
        }
    }
}