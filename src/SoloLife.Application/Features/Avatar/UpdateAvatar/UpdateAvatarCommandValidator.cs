namespace SoloLife.Application.Features.Avatar.UpdateAvatar;

using FluentValidation;

public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
{
    public UpdateAvatarCommandValidator()
    {
        RuleFor(x => x.CurrentSkin)
            .NotEmpty().WithMessage("Skin é obrigatória.")
            .MaximumLength(100).WithMessage("Skin deve ter no máximo 100 caracteres.");

        RuleFor(x => x.CurrentBackground)
            .NotEmpty().WithMessage("Background é obrigatório.")
            .MaximumLength(100).WithMessage("Background deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Accessories)
            .NotNull().WithMessage("Acessórios não podem ser nulos.");
    }
}
