using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Entities.Warships;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.ViewModels;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.PlaceWarship
{
    public class PlaceWarshipCommand : IRequest<IValidationResult<PlayerGameViewModel>>
    {
        public Guid GameId { get; init; }

        public Guid PlayerId { get; init; } = default!;

        public Warship Warship { get; init; } = default!;

        public Placement? Placement { get; init; }
    }
}
