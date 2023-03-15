using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Features.GameSetup.Commands.JoinGame;
using BattleshipGame.Core.Application.Features.GameSetup.Commands.PlaceWarship;
using BattleshipGame.Core.Application.Features.Gameplay.Commands.MakeShot;
using BattleshipGame.Core.Application.Features.Gameplay.Queries.GetGameState;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Domain.Entities;
using MediatR;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Abstractions.Randomization;

namespace BattleshipGame.Infrastructure.Players.Computer
{
    internal class ComputerPlayerService : INotificationHandler<GameCreatedEvent>, INotificationHandler<PlayerTurnEvent>
    {
        private readonly Player _player = new Player { Id = Guid.NewGuid(), Name = "Computer" };
        private readonly IGameSettings _gameSettings;
        private readonly IMediator _mediator;
        private readonly IRandomizer _randomizer;

        public ComputerPlayerService(
            IGameSettings gameSettings,
            IMediator mediator,
            IRandomizer randomizer)
        {
            _gameSettings = gameSettings;
            _mediator = mediator;
            _randomizer = randomizer;
        }

        public async Task Handle(GameCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _mediator.Send(new JoinGameCommand { GameId = notification.GameId, Player = _player }, cancellationToken);
            foreach (var warship in _gameSettings.BattlefieldWarships)
            {
                await _mediator.Send(new PlaceWarshipCommand
                {
                    GameId = notification.GameId,
                    PlayerId = _player.Id,
                    Warship = warship
                });
            }
        }

        public async Task Handle(PlayerTurnEvent notification, CancellationToken cancellationToken)
        {
            if (notification.PlayerId == _player.Id)
            {
                var requestResult = await _mediator.Send(
                    new GetGameStateQuery { GameId = notification.GameId, PlayerId = _player.Id }, cancellationToken);
                var range = EnumerateField(requestResult.Value.OpponentBattlefield.CellStates)
                    .Select((v, i) => (CellState: v, Index: i))
                    .Where(x => x.CellState == BattlefieldCellState.Water || x.CellState == BattlefieldCellState.Deck)
                    .Select(x => x.Index)
                    .ToList();
                await _mediator.Send(new MakeShotCommand
                {
                    GameId = notification.GameId,
                    Player = _player,
                    Point = Position.FromIndex(range[_randomizer.GetNext(range.Count)], _gameSettings.BattlefieldSize)
                }, cancellationToken);
            }
        }

        private static IEnumerable<T> EnumerateField<T>(T[,] field)
        {
            for (var i = 0; i < field.GetLength(0); i++)
                for (var j = 0; j < field.GetLength(1); j++)
                {
                    yield return field[i, j];
                }
        }
    }
}