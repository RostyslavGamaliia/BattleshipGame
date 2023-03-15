using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.ViewModels.Factories
{
    internal class PlayerGameViewModelFactory : IPlayerGameViewModelFactory
    {
        private readonly IBattlefieldViewModelFactory _battlefieldViewModelFactory;

        public PlayerGameViewModelFactory(IBattlefieldViewModelFactory battlefieldViewModelFactory)
        {
            _battlefieldViewModelFactory = battlefieldViewModelFactory;
        }

        public PlayerGameViewModel Create(Game game, int currentPlayerIndex)
        {
            return new PlayerGameViewModel
            {
                PlayerBattlefield = _battlefieldViewModelFactory.Create(game.Battlefields[currentPlayerIndex], true),
                OpponentBattlefield = _battlefieldViewModelFactory.Create(game.Battlefields[1 - currentPlayerIndex], false),
                GameTurn = game.CurrentTurn,
                IsPlayersTurn = game.CurrentTurn == currentPlayerIndex
            };
        }
    }
}
