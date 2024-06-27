using FluentValidation;
using Project.RequestModels;

namespace Project.Validators;

public class UpdateBusinessClientValidator : AbstractValidator<UpdateBusinessClientModel>
{
    public UpdateBusinessClientValidator()
    {
        RuleFor(e => e.Name)
            .MaximumLength(50)
            .When(e => !string.IsNullOrEmpty(e.Name));

        RuleFor(e => e.Email)
            .MaximumLength(50)
            .EmailAddress()
            .When(e => !string.IsNullOrEmpty(e.Email));;

        RuleFor(e => e.Address)
            .MaximumLength(50)
            .When(e => !string.IsNullOrEmpty(e.Address));
        
        RuleFor(e => e.Phone)
            .Length(9)
            .Matches(@"^\d+$")
            .WithMessage("Phone number must contain only digits")
            .When(e => !string.IsNullOrEmpty(e.Phone));
    }
}