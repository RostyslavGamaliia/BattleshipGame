using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Core.Application.Abstractions.Entities.Positioning;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Features.GameSetup.Commands.CreateGame;
using BattleshipGame.Core.Application.Features.GameSetup.Commands.PlaceWarship;
using BattleshipGame.Core.Application.Features.GameSetup.Commands.StartGame;
using BattleshipGame.Core.Application.Features.Gameplay.Commands.MakeShot;
using BattleshipGame.Core.Application.Features.Gameplay.Queries.GetGameState;
using BattleshipGame.Infrastructure.Players.Services;
using BattleshipGame.Core.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace BattleshipGame.Infrastructure.Players.Current
{
    internal class CurrentPlayerService : IHostedService,
        INotificationHandler<PlayerReadyEvent>,
        INotificationHandler<PlayerTurnEvent>,
        INotificationHandler<GameEndedEvent>
    {
        private readonly IGameSettings _gameSettings;
        private readonly IUIInteractionService _uiService;
        private readonly IMediator _mediator;
        private readonly Guid _gameId = Guid.NewGuid();
        private Player _player = default!;

        public CurrentPlayerService(IGameSettings gameSettings, IUIInteractionService uiService, IMediator mediator)
        {
            _gameSettings = gameSettings;
            _uiService = uiService;
            _mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _uiService.DrawStaticView(_gameSettings.BattlefieldSize);
            _uiService.SetStatusText("Welcome to the battleship game! Please enter your name:");
            _player = new Player
            {
                Id = Guid.NewGuid(),
                Name = await _uiService.ReadUserInputAsync()
            };

            var creationResult = await _mediator.Send(new CreateGameCommand
            {
                GameId = _gameId,
                Player = _player
            }, cancellationToken).ConfigureAwait(false);
            _uiService.DrawFrame(creationResult.Value);
        }

        public async Task Handle(PlayerReadyEvent notification, CancellationToken cancellationToken)
        {
            if (notification.GameId != _gameId)
            {
                return;
            }

            if (notification.PlayerId == _player.Id)
            {
                await _mediator.Send(new StartGameCommand { GameId = notification.GameId }, cancellationToken);
            }
            else
            {
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
        }

        public async Task Handle(PlayerTurnEvent notification, CancellationToken cancellationToken)
        {
            if (notification.GameId == _gameId && notification.PlayerId == _player.Id)
            {
                var requestResult = await _mediator.Send(new GetGameStateQuery { GameId = _gameId, PlayerId = _player.Id });
                _uiService.DrawFrame(requestResult.Value);

                _uiService.SetStatusText($"{_player.Name}, it is your turn now:");
                Position position;
                while (!Position.TryParse(await _uiService.ReadUserInputAsync(), out position, _gameSettings.BattlefieldSize))
                {
                    _uiService.SetStatusText($"{_player.Name}, please enter correct battlefield coordinates in a format `A7`:");
                }
                var makeShotResult = await _mediator.Send(new MakeShotCommand { GameId = _gameId, Player = _player, Point = position });
                _uiService.DrawFrame(makeShotResult.Value);
            }
        }

        public Task Handle(GameEndedEvent notification, CancellationToken cancellationToken)
        {
            if (notification.GameId == _gameId)
            {
                if (notification.Winner.Id == _player.Id)
                {
                    _uiService.SetStatusText($"Congrats, {_player.Name}!!! Your opponent defeated!");
                }
                else
                {
                    _uiService.SetStatusText($"Sorry, {_player.Name}. {notification.Winner.Name} beat you this time...");
                }
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _uiService.SetStatusText("Disconnecting from game");
            return Task.CompletedTask;
        }
    }
}