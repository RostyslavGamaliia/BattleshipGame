namespace BattleshipGame.Core.Application.ViewModels
{
    public record PlayerGameViewModel
    {
        public bool IsPlayersTurn { get; init; }

        public int GameTurn { get; init; }

        public BattlefieldViewModel PlayerBattlefield { get; init; } = default!;

        public BattlefieldViewModel OpponentBattlefield { get; init; } = default!;
    }
}
