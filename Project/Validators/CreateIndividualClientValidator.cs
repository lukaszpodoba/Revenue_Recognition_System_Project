using FluentValidation;
using Project.RequestModels;

namespace Project.Validators;

public class CreateIndividualClientValidator : AbstractValidator<CreateIndividualClientModel>
{
    public CreateIndividualClientValidator()
    {
        RuleFor(e => e.FirstName)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(e => e.LastName)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(e => e.PESEL)
            .Length(11)
            .Matches(@"^\d+$")
            .WithMessage("PESEL must contain only digits");
        
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