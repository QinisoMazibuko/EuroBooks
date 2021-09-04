using FluentValidation;

namespace EuroBooks.Application.Subscribtion.Commands
{
    class UnsubscribeCommandValidator : AbstractValidator<UnsubscribeCommand>
    {
        public UnsubscribeCommandValidator()
        {
            RuleFor(v => v.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(v => v.Id).NotEmpty().WithMessage("Subscription Id is required");
        }
    }
}
