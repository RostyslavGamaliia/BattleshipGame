using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Entities.Warships;
using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.Abstractions.Validation
{
    public interface IWarshipPlacementValidation
    {
        bool CanAddWarship(Battlefield battlefield, Warship warship);

        WarshipPlacement? TryPlaceWarship(Battlefield battlefield, Warship warship, Placement placement);
    }
}
