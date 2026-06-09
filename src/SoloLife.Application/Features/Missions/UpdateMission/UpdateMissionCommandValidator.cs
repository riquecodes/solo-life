namespace SoloLife.Application.Features.Missions.UpdateMission;

using FluentValidation;

public class UpdateMissionCommandValidator : AbstractValidator<UpdateMissionCommand>
{
    public UpdateMissionCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .MaximumLength(120).WithMessage("Título deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Categoria inválida.");

        RuleFor(x => x.XpReward)
            .GreaterThan(0).WithMessage("XP deve ser maior que zero.");
    }
}
