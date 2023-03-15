using BattleshipGame.Core.Application.Abstractions.Validation;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.StartGame
{
    public class StartGameCommand : IRequest<IValidationResult>
    {
        public Guid GameId { get; init; }
    }
}
