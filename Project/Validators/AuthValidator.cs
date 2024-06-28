using FluentValidation;
using Project.RequestModels;

namespace Project.Validators;

public class AuthValidator : AbstractValidator<LoginAndRegisterRequestModel>
{
    public AuthValidator()
    {
        RuleFor(e => e.Password).NotEmpty();
        RuleFor(e => e.UserName).NotEmpty();
    }
}