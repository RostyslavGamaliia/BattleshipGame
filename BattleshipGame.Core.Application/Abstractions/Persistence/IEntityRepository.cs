using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.Abstractions.Persistence
{
    public interface IEntityRepository<T> where T : IEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(T entity, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task SaveChangesAsync();
    }
}
