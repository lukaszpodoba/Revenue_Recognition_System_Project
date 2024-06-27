using FluentValidation;
using Project.RequestModels;

namespace Project.Validators;

public class CreateBusinessClientValidator : AbstractValidator<CreateBusinessClientModel>
{
    public CreateBusinessClientValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(e => e.KRS)
            .Must(krs => krs.Length is 9 or 14)
            .WithMessage("KRS number must be 9 or 14 characters long")
            .Matches(@"^\d+$")
            .WithMessage("KRS number must contain only digits");

        RuleFor(e => e.Email)
            .NotEmpty()
            .MaximumLength(50)
            .EmailAddress();

        RuleFor(e => e.Address)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(e => e.Phone)
            .Length(9)
            .Matches(@"^\d+$")
            .WithMessage("Phone number must contain only digits");
    }
}