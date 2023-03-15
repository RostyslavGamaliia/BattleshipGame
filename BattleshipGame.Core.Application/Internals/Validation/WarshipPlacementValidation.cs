using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Entities.Warships;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.Internals.Validation
{
    internal class WarshipPlacementValidation : IWarshipPlacementValidation
    {
        private readonly IGameSettings _gameSettings;

        public WarshipPlacementValidation(IGameSettings gameSettings)
        {
            _gameSettings = gameSettings;
        }

        public bool CanAddWarship(Battlefield battlefield, Warship warship)
        {
            var currentWarshipSizes = battlefield.WarshipsPlacement
                .Select(wp => (
                    Start: Position.FromIndex(wp.StartCellIndex, _gameSettings.BattlefieldSize),
                    End: Position.FromIndex(wp.EndCellIndex, _gameSettings.BattlefieldSize)))
                .Select(x => Math.Abs(x.Start.Top - x.End.Top) + Math.Abs(x.Start.Left - x.End.Left) + 1);
            var allowedWarshipSizes = _gameSettings.BattlefieldWarships
                .GroupBy(x => x.Length)
                .ToDictionary(x => x.Key, x => x.Count());
            foreach (var warshipSize in currentWarshipSizes.Concat(Enumerable.Repeat(warship.Length, 1)))
            {
                if (!allowedWarshipSizes.TryGetValue(warshipSize, out var count) || count <= 0)
                {
                    return false;
                }
                allowedWarshipSizes[warshipSize] = count - 1;
            }
            return true;
        }

        public WarshipPlacement? TryPlaceWarship(Battlefield battlefield, Warship warship, Placement placement)
        {
            var distTolerance = _gameSettings.WarshipsCanTouch ? 0 : 1;
            var warshipPlacement = PlacementToWarshipPlacement(warship, placement);
            return battlefield.WarshipsPlacement.All(x => !IsIntersect(x, warshipPlacement, distTolerance))
                ? warshipPlacement
                : null;
        }

        private WarshipPlacement PlacementToWarshipPlacement(Warship warship, Placement placement)
        {
            return new WarshipPlacement
            {
                StartCellIndex = placement.Position.GetIndexPosition(_gameSettings.BattlefieldSize),
                EndCellIndex = placement.GetEndPosition(warship.Length).GetIndexPosition(_gameSettings.BattlefieldSize)
            };
        }

        private bool IsIntersect(WarshipPlacement placement1, WarshipPlacement placement2, int distTolerance)
        {
            foreach (var pos1 in EnumeratePlacement(placement1))
                foreach (var pos2 in EnumeratePlacement(placement2))
                {
                    if (Math.Abs(pos1.Left - pos2.Left) <= distTolerance && Math.Abs(pos1.Top - pos2.Top) <= distTolerance)
                    {
                        return true;
                    }
                }
            return false;
        }

        private IEnumerable<Position> EnumeratePlacement(WarshipPlacement placement)
        {
            return placement
                .GetAllIndexes(_gameSettings.BattlefieldSize)
                .Select(i => Position.FromIndex(i, _gameSettings.BattlefieldSize));
        }
    }
}
