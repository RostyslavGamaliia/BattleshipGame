using BattleshipGame.Core.Application.ViewModels;

namespace BattleshipGame.Infrastructure.Players.Services
{
    public interface IUIInteractionService
    {
        Task<string> ReadUserInputAsync();

        void SetStatusText(string statusText);

        void DrawStaticView(int fieldSize);

        void DrawFrame(PlayerGameViewModel playerGameViewModel);
    }
}