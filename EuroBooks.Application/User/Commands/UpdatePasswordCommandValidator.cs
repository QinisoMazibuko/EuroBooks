using FluentValidation;

namespace EuroBooks.Application.User.Commands
{
    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator()
        {
            RuleFor(v => v.UserID).GreaterThanOrEqualTo(0).WithMessage("Not a valid user id");
            RuleFor(v => v.PasswordHash).NotEmpty().WithMessage("Password is required.");
        }
    }
}
