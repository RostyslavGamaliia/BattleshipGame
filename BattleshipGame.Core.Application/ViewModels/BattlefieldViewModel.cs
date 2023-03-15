namespace BattleshipGame.Core.Application.ViewModels
{
    public record BattlefieldViewModel
    {
        public BattlefieldCellState[,] CellStates { get; init; } = default!;
    }
}
