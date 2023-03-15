using BattleshipGame.Core.Application;
using BattleshipGame.Infrastructure.Persistence;
using BattleshipGame.Infrastructure.Players;
using BattleshipGame.Infrastructure.Players.Services;
using BattleshipGame.UI.Console.ConsoleUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BattleshipGame.UI.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(b => b.ClearProviders())
                .ConfigureServices(services =>
                {
                    services.AddApplicationServices();
                    services.AddPersistence();
                    services.AddComputerPlayer();
                    services.AddInteractivePlayer();

                    services.AddTransient<IActionsQueue, ActionsQueue>();
                    services.AddSingleton<IUIInteractionService, ConsoleUIService>();
                })
                .UseConsoleLifetime()
                .Build();
            host.Run();
        }
    }
}