using FluentValidation;
using Sample.Common.DTOs.Requests;

namespace Sample.Common.DTOs.UserValidator
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(r => r.Username)
                .NotEmpty()
                .WithMessage("The username field is required.");

            RuleFor(r => r.FirstName)
                .NotEmpty()
                .WithMessage("The firstname field is required.");

            RuleFor(r => r.LastName)
                .NotEmpty()
                .WithMessage("The lastname field is required.");

            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).+$")
                .WithMessage("The password must contain at least one uppercase letter, one lowercase letter, and one special character, and be at least 8 characters long.");
        }
    }
}