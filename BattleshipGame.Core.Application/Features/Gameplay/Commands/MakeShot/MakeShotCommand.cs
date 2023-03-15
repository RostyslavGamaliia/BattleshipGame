using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.Gameplay.Commands.MakeShot
{
    public class MakeShotCommand : IRequest<IValidationResult<PlayerGameViewModel>>
    {
        public Guid GameId { get; set; }

        public Player Player { get; set; } = default!;

        public Position Point { get; set; }
    }
}
