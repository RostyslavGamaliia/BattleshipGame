using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Abstractions.Events
{
    public record GameEndedEvent : INotification
    {
        public Guid GameId { get; init; }

        public Player Winner { get; init; } = default!;
    }
}
