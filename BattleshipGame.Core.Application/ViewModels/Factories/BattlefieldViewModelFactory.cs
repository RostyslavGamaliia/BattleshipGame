using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.ViewModels.Factories
{
    internal class BattlefieldViewModelFactory : IBattlefieldViewModelFactory
    {
        public BattlefieldViewModel Create(Battlefield battlefield, bool showWarships)
        {
            var warshipCellIndexes = battlefield.WarshipsPlacement.SelectMany(wp => wp.GetAllIndexes(battlefield.Size)).ToHashSet();
            return new BattlefieldViewModel
            {
                CellStates = CreateCellStates(battlefield.ShotsMap, warshipCellIndexes, battlefield.Size, showWarships)
            };
        }

        private static BattlefieldCellState[,] CreateCellStates(
            IReadOnlyList<bool> shotsMap, HashSet<int> warshipCellIndexes, int fieldSize, bool showWarships)
        {
            var cellStates = new BattlefieldCellState[fieldSize, fieldSize];
            for (var i = 0; i < shotsMap.Count; i++)
            {
                var position = Position.FromIndex(i, fieldSize);
                cellStates[position.Top, position.Left] = shotsMap[i]
                    ? warshipCellIndexes.Contains(i) ? BattlefieldCellState.Hit : BattlefieldCellState.Miss
                    : showWarships && warshipCellIndexes.Contains(i) ? BattlefieldCellState.Deck : BattlefieldCellState.Water;
            }
            return cellStates;
        }
    }
}
