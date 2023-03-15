using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Infrastructure.Players.Computer;
using BattleshipGame.Infrastructure.Players.Current;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BattleshipGame.Infrastructure.Players
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddComputerPlayer(this IServiceCollection services)
        {
            services.AddSingleton<ComputerPlayerService>();
            services.AddTransient<INotificationHandler<GameCreatedEvent>>(sp => sp.GetRequiredService<ComputerPlayerService>());
            services.AddTransient<INotificationHandler<PlayerTurnEvent>>(sp => sp.GetRequiredService<ComputerPlayerService>());
            return services;
        }

        public static IServiceCollection AddInteractivePlayer(this IServiceCollection services)
        {
            services.AddSingleton<CurrentPlayerService>();
            services.AddHostedService(sp => sp.GetRequiredService<CurrentPlayerService>());
            services.AddTransient<INotificationHandler<PlayerReadyEvent>>(sp => sp.GetRequiredService<CurrentPlayerService>());
            services.AddTransient<INotificationHandler<PlayerTurnEvent>>(sp => sp.GetRequiredService<CurrentPlayerService>());
            services.AddTransient<INotificationHandler<GameEndedEvent>>(sp => sp.GetRequiredService<CurrentPlayerService>());
            return services;
        }
    }
}
