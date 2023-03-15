using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Domain.Entities;
using BattleshipGame.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace BattleshipGame.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddTransient<IEntityRepository<Game>, InMemoryRepository<Game>>();
            return services;
        }
    }
}
