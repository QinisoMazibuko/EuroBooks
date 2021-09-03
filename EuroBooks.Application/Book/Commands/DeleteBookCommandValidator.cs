using FluentValidation;

namespace EuroBooks.Application.Book.Commands
{
    class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
    {
        public DeleteBookCommandValidator()
        {
            RuleFor(v => v.Id).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Book ID is required");
        }
    }
}
