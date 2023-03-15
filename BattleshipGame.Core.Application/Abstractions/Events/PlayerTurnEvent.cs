using MediatR;

namespace BattleshipGame.Core.Application.Abstractions.Events
{
    public record PlayerTurnEvent : INotification
    {
        public Guid GameId { get; init; }

        public Guid PlayerId { get; init; }
    }
}
