using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.JoinGame
{
    public class JoinGameCommand : IRequest<IValidationResult<PlayerGameViewModel>>
    {
        public Guid GameId { get; init; }

        public Player Player { get; init; } = default!;
    }
}
