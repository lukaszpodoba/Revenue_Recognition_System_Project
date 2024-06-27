using FluentValidation;
using Project.RequestModels;

namespace Project.Validators;

public class CreateAgreementValidator : AbstractValidator<CreateAgreementModel>
{
    public CreateAgreementValidator()
    {
        RuleFor(e => e.PaymentFrom)
            .NotEmpty();
        
        RuleFor(e => e.PaymentUntil)
            .NotEmpty()
            .Must((model, paymentUntil) => ValidatePaymentUntil(model.PaymentFrom, paymentUntil))
            .WithMessage("Payment duration must be between 3 and 30 days");

        RuleFor(e => e.YearsOfVersionSupport)
            .NotEmpty()
            .GreaterThan(1)
            .WithMessage("Minimal number of years of version support is 1");
    }
    
    private bool ValidatePaymentUntil(DateTime paymentFrom, DateTime paymentUntil)
    {
        var duration = paymentUntil - paymentFrom;
        return duration.TotalDays is >= 3 and <= 30;
    }
}