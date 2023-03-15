using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Abstractions.Events
{
    public record GameCreatedEvent : INotification
    {
        public Guid GameId { get; init; }

        public Player Creator { get; init; } = default!;
    }
}
