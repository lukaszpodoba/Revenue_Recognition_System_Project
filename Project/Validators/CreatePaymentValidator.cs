using FluentValidation;
using Project.RequestModels;

namespace Project.Validators;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentModel>
{
    public CreatePaymentValidator()
    {
        RuleFor(e => e.DepositSize)
            .GreaterThan(0)
            .WithMessage("Deposit size must be greater than 0");
    }
}