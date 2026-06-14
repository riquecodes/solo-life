namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IAvatarRepository
{
    Task<Avatar?> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    void Update(Avatar avatar);
}
