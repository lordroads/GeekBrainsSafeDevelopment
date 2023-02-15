using CardStorageService.Models.Requests;
using FluentValidation;

namespace CardStorageService.Models.Validators;

public class CreateCardRequestValidation : AbstractValidator<CreateCardRequest>
{
    public CreateCardRequestValidation()
    {
        RuleFor(x => x.CardNo)
            .Length(16, 20);

        RuleFor(x => x.Name)
            .Length(5, 50);

        RuleFor(x => x.CVV2)
            .Length(3, 50);

        RuleFor(x => x.ExpDate)
            .NotEmpty()
            .NotNull()
            .GreaterThan(DateTime.Now.AddDays(-1))
            .WithMessage("���� �������� ����� ����� �������.");
    }
}