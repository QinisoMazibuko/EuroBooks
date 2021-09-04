using FluentValidation;

namespace EuroBooks.Application.Subscribtion.Commands
{
    public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
    {
        public CreateSubscriptionCommandValidator()
        {
            RuleFor(v => v.UserId).NotEmpty().WithMessage("UserId is required.");
            RuleFor(v => v.BookId).NotEmpty().WithMessage("BookId is required");  
        }
    }
}
