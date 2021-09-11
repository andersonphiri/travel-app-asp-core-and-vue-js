using Travel.Application.Common.Interfaces;
using FluentValidation;

namespace Travel.Application.TourLists.Commands.CreateTourList
{
    public class CreateTourListCommandValidator : AbstractValidator<CreateTourListCommand>
    {
        private readonly IApplicationDbContext _context;
        public CreateTourListCommandValidator(IApplicationDbContext context)
        {
            _context = context;
            RuleFor(v => v.City)
                .NotEmpty()
                .WithMessage("City is required.")
                .MaximumLength(200)
                .WithMessage("City must not exceed 200 characters");

            RuleFor(v => v.Country)
                .NotEmpty().WithMessage("country is required")
                .MaximumLength(200).WithMessage("country must not exceed 200 characters");
            RuleFor(v => v.About)
                .NotEmpty().WithMessage("About is required");
        }
    }

}
