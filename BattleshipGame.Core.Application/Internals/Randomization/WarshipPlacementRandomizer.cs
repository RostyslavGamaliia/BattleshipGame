using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Entities.Warships;
using BattleshipGame.Core.Application.Abstractions.Randomization;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.Internals.Randomization
{
    internal class WarshipPlacementRandomizer : IWarshipPlacementRandomizer
    {
        private readonly IRandomizer _randomizer;
        private readonly IWarshipPlacementValidation _warshipPlacementValidation;

        public WarshipPlacementRandomizer(IRandomizer randomizer, IWarshipPlacementValidation warshipPlacementValidation)
        {
            _randomizer = randomizer;
            _warshipPlacementValidation = warshipPlacementValidation;
        }

        public Placement GetPlacement(Battlefield battlefield, Warship warship)
        {
            Placement? placement = null;
            while (placement is null)
            {
                var randomDirection = _randomizer.GetNext(2) == 0 ? Direction.Horizontal : Direction.Vertical;
                var randomPosShort = _randomizer.GetNext(battlefield.Size - warship.Length + 1);
                var randomPosWide = _randomizer.GetNext(battlefield.Size);
                var randomPlacement = new Placement
                {
                    Direction = randomDirection,
                    Position = new Position
                    {
                        Left = randomDirection == Direction.Horizontal ? randomPosShort : randomPosWide,
                        Top = randomDirection == Direction.Horizontal ? randomPosWide : randomPosShort,
                    }
                };

                if (_warshipPlacementValidation.TryPlaceWarship(battlefield, warship, randomPlacement).HasValue)
                {
                    placement = randomPlacement;
                }
            }

            return placement.Value;
        }
    }
}
