namespace SoloLife.Application.Features.Missions.CreateMission;

using FluentValidation;

public class CreateMissionCommandValidator : AbstractValidator<CreateMissionCommand>
{
    public CreateMissionCommandValidator()
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
