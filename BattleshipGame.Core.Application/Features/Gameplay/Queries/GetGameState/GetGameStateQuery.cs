using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.ViewModels;
using MediatR;

namespace BattleshipGame.Core.Application.Features.Gameplay.Queries.GetGameState
{
    public class GetGameStateQuery : IRequest<IValidationResult<PlayerGameViewModel>>
    {
        public Guid GameId { get; init; }

        public Guid PlayerId { get; init; } = default!;
    }
}
