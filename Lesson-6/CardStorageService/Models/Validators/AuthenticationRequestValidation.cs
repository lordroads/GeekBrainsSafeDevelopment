using CardStorageService.Models.Requests;
using FluentValidation;

namespace CardStorageService.Models.Validators;

public class AuthenticationRequestValidation : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidation()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .NotNull()
            .Length(5, 255)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .Length(5, 50);
    }
}