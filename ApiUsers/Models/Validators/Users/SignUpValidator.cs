using ApiUsers.Models.Requests;
using FluentValidation;
using FluentValidation.Validators;

namespace ApiUsers.Models.Validators.Users
{
    public class SignUpValidator : AbstractValidator<SignUpRequest>
    {
        public SignUpValidator()
        {
            RuleFor(r => r.FirstName)
                .NotNull().WithMessage("Fist name cannot be null")
                .NotEmpty().WithMessage("Need provide a fisrt name");

            RuleFor(r => r.LastName)
                .NotNull().WithMessage("Last name cannot be null")
                .NotEmpty().WithMessage("Need provide a last name");

            RuleFor(r => r.Email)
                .NotNull().WithMessage("Email cannot be null")
                .NotEmpty().WithMessage("Need provide a email")
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotNull().WithMessage("Password cannot be null")
                .NotEmpty().WithMessage("Need provide a password")
                .Matches(@"^(?=.*[a-z]){3}(?=.*[A-Z]){3}(?=.*\d){2}(?=.*[\W@$!%*?&]){2}[A-Za-z\d@$!%*?&]{10}$")
                .WithMessage("The password must contain at least 2 numbers, 2 special characters, lowercase and uppercase letters.");
        }
    }
}
