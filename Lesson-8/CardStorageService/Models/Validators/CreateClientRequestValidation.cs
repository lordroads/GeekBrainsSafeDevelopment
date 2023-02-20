using CardStorageService.Models.Requests;
using FluentValidation;

namespace CardStorageService.Models.Validators;

public class CreateClientRequestValidation : AbstractValidator<CreateClientRequest>
{
    public CreateClientRequestValidation()
    {
        RuleFor(x => x.FirstName)
            .Length(3, 255);

        RuleFor(x => x.Surname)
            .Length(3, 255);

        RuleFor(x => x.Patronymic)
            .Length(3, 255);
    }
}

public class CreateClientRequestValidationProto : AbstractValidator<CardStorageServiceProtos.CreateClientRequest>
{
    public CreateClientRequestValidationProto()
    {
        RuleFor(x => x.FirstName)
            .Length(3, 255);

        RuleFor(x => x.Surname)
            .Length(3, 255);

        RuleFor(x => x.Patronymic)
            .Length(3, 255);
    }
}