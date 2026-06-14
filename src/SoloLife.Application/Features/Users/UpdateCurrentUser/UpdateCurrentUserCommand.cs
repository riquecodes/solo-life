namespace SoloLife.Application.Features.Users.UpdateCurrentUser;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Users.Dtos;

public record UpdateCurrentUserCommand(string UserId, string Name)
    : IRequest<Result<UserDto>>;

public class UpdateCurrentUserCommandHandler
    : IRequestHandler<UpdateCurrentUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCurrentUserCommandHandler(IUserRepository users, IUnitOfWork unitOfWork)
    {
        _users = users;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<UserDto>("Usuário não encontrado.");

        user.Name = request.Name.Trim();

        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CurrentLevel,
            user.CurrentXp,
            user.CurrentStreak,
            user.CreatedAt));
    }
}
