namespace ApiUsers.Models.Validators.Users
{
    public class InsertValidator : AbstractValidator<InsertRequest>
    {
        public InsertValidator()
        {
            RuleFor(r => r.FirstName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Need provide a fisrt name");

            RuleFor(r => r.LastName)
                .NotNull()
                .NotEmpty()
                .WithMessage("Need provide a last name");

            RuleFor(r => r.Email)
                .NotNull() 
                .NotEmpty()
                .WithMessage("Need provide a email")
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotNull()
                .NotEmpty()
                .WithMessage("Need provide a password")
                .Matches(@"^(?=.*[a-z]){3}(?=.*[A-Z]){3}(?=.*\d){2}(?=.*[\W@$!%*?&]){2}[A-Za-z\d@$!%*?&]{10}$")
                .WithMessage("The password must contain at least 2 numbers, 2 special characters, lowercase and uppercase letters.");
        }
    }
}
