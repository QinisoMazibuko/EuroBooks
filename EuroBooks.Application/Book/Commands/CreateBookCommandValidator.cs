using FluentValidation;

namespace EuroBooks.Application.Book.Commands
{
    class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateBookCommandValidator()
        {
            RuleFor(v => v.Name).NotEmpty().WithMessage("Book name is required.");
            RuleFor(v => v.Text).NotEmpty().WithMessage("Not a valid Book Description");
            RuleFor(v => v.PurchasePrice).NotEmpty().GreaterThanOrEqualTo(1).WithMessage("Book price is required");
        }
    }
}
