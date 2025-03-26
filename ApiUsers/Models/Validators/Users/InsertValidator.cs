using ApiUsers.Models.Requests.Uers;
using FluentValidation;

namespace ApiUsers.Models.Validators.Users
{
    public class InsertValidator : AbstractValidator<InsertRequest>
    {
        public InsertValidator()
        {
            //TODO. Validate if required field is equals to not null or only mean this field is necesary in the model, the value can be null, is this way, add the null rule again.
            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("Need provide a fisrt name");

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("Need provide a last name");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Need provide a email")
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Need provide a password")
                .Matches(@"^(?=.*[a-z]){3}(?=.*[A-Z]){3}(?=.*\d){2}(?=.*[\W@$!%*?&]){2}[A-Za-z\d@$!%*?&]{10}$")
                .WithMessage("The password must contain at least 2 numbers, 2 special characters, lowercase and uppercase letters.");
        }
    }
}
