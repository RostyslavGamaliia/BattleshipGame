using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Application.ViewModels.Factories;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.CreateGame
{
    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, IValidationResult<PlayerGameViewModel>>
    {
        private readonly IEntityRepository<Game> _gameRepository;
        private readonly IPublisher _eventPublisher;
        private readonly IGameSettings _gameSettings;
        private readonly IPlayerGameViewModelFactory _playerGameViewModelFactory;

        public CreateGameCommandHandler(
            IEntityRepository<Game> gameRepository,
            IPublisher eventPublisher,
            IGameSettings gameSettings,
            IPlayerGameViewModelFactory playerGameViewModelFactory)
        {
            _gameRepository = gameRepository;
            _eventPublisher = eventPublisher;
            _gameSettings = gameSettings;
            _playerGameViewModelFactory = playerGameViewModelFactory;
        }

        public async Task<IValidationResult<PlayerGameViewModel>> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {
            var game = new Game
            {
                Id = request.GameId,
                BattlefieldSize = _gameSettings.BattlefieldSize,
            };
            game.Players[0] = request.Player;
            for (var i = 0; i <= 1; i++)
            {
                game.Battlefields[i] = new Battlefield(_gameSettings.BattlefieldSize);
            }
            await _gameRepository.CreateAsync(game, cancellationToken);
            await _gameRepository.SaveChangesAsync();

            _ = _eventPublisher.Publish(new GameCreatedEvent { GameId = request.GameId, Creator = request.Player });

            return new ValidationResult<PlayerGameViewModel>(_playerGameViewModelFactory.Create(game, 0));
        }
    }
}
