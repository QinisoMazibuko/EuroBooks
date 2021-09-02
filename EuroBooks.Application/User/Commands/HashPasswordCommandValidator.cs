using FluentValidation;

namespace EuroBooks.Application.User.Commands
{
    public class HashPasswordCommandValidator : AbstractValidator<HashPasswordCommand>
    {
        public HashPasswordCommandValidator()
        {
            RuleFor(v => v.UserID).GreaterThanOrEqualTo(0).WithMessage("Not a valid user id");
            RuleFor(v => v.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}
