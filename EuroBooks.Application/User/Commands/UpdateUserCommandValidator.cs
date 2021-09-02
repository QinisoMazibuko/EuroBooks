using FluentValidation;

namespace EuroBooks.Application.User.Commands
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(v => v.UserId).GreaterThanOrEqualTo(0).WithMessage("Not a valid user id");
            RuleFor(v => v.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(v => v.LastName).NotEmpty().WithMessage("Last name is required.");
        }
    }
}
