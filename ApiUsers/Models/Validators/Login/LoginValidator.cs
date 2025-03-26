using ApiUsers.Models.Requests;
using FluentValidation;

namespace ApiUsers.Models.Validators.Login
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Need provide a email")
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Need provide a password");
        }
    }
}
