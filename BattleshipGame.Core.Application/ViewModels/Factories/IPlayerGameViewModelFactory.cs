using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.ViewModels.Factories
{
    public interface IPlayerGameViewModelFactory
    {
        PlayerGameViewModel Create(Game game, int currentPlayerIndex);
    }
}
