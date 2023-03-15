using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Entities.Warships;
using BattleshipGame.Core.Domain.Entities;

namespace BattleshipGame.Core.Application.Abstractions.Randomization
{
    public interface IWarshipPlacementRandomizer
    {
        Placement GetPlacement(Battlefield battlefield, Warship warship);
    }
}
