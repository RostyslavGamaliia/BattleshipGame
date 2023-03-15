using BattleshipGame.Core.Application.Abstractions.Randomization;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Randomization;
using BattleshipGame.Core.Application.Internals.Settings;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Application.ViewModels.Factories;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BattleshipGame.Core.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient<IBattlefieldViewModelFactory, BattlefieldViewModelFactory>();
            services.AddTransient<IPlayerGameViewModelFactory, PlayerGameViewModelFactory>();
            services.AddTransient<IGameSettings, GameSettings>();
            services.AddTransient<IWarshipPlacementValidation, WarshipPlacementValidation>();
            services.AddTransient<IRandomizer, Randomizer>();
            services.AddTransient<IWarshipPlacementRandomizer, WarshipPlacementRandomizer>();
            return services;
        }
    }
}
