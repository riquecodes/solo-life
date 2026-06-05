namespace SoloLife.Application.Features.Missions.DeleteMission;

using MediatR;
using SoloLife.Application.Common.Results;

public record DeleteMissionCommand(Guid Id, Guid UserId)
    : IRequest<Result>;

public class DeleteMissionCommandHandler
    : IRequestHandler<DeleteMissionCommand, Result>
{
    public Task<Result> Handle(DeleteMissionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure("Não implementado."));
}
