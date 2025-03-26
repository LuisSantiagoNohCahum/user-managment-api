using ApiUsers.Models.Requests;
using FluentValidation;

namespace ApiUsers.Models.Validators.Users
{
    public class GetAllValidator : AbstractValidator<GetAllRequest>
    {
        public GetAllValidator()
        {
            RuleFor(r => r.Status)
                .NotNull().WithMessage("Status cannot be null")
                .NotEmpty().WithMessage("Need provide a Status");
        }
    }
}
