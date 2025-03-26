using ApiUsers.Models.Requests.Uers;
using FluentValidation;

namespace ApiUsers.Models.Validators.Users
{
    public class UpdateValidator : AbstractValidator<UpdateRequest>
    {
        public UpdateValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("Need provide a fisrt name");

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("Need provide a last name");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Need provide a email")
                .EmailAddress();

            RuleFor(x => x.RolId).GreaterThan(0);
        }
    }
}
