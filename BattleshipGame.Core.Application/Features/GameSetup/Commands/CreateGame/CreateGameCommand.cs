using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.CreateGame
{
    public class CreateGameCommand : IRequest<IValidationResult<PlayerGameViewModel>>
    {
        public Guid GameId { get; init; }

        public Player Player { get; init; } = default!;
    }
}
