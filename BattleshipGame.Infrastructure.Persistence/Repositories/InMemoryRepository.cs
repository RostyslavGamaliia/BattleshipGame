using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Domain.Entities;
using System.Collections.Concurrent;

namespace BattleshipGame.Infrastructure.Persistence.Repositories
{
    internal class InMemoryRepository<T> : IEntityRepository<T> where T : IEntity
    {
        private static readonly ConcurrentDictionary<Guid, T> _inMemoryCache = new();

        private readonly ConcurrentDictionary<Guid, T> _updateCache = new();
        private readonly ConcurrentBag<Guid> _removeCache = new();

        public Task<bool> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_updateCache.TryAdd(entity.Id, entity));
        }

        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _removeCache.Add(entity.Id);
            return Task.CompletedTask;
        }

        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_inMemoryCache.TryGetValue(id, out var entity) ? entity : default);
        }

        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_updateCache.AddOrUpdate(entity.Id, id => entity, (id, _) => entity));
        }

        public Task SaveChangesAsync()
        {
            lock (_inMemoryCache)
            {
                foreach (var (id, entity) in _updateCache)
                {
                    _inMemoryCache.AddOrUpdate(id, _ => entity, (_, _) => entity);
                }
                foreach (var id in _removeCache)
                {
                    _inMemoryCache.TryRemove(id, out var _);
                }
            }
            return Task.CompletedTask;
        }
    }
}
