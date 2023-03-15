using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Application.Abstractions.Randomization;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.StartGame
{
    public class StartGameCommandHandler : IRequestHandler<StartGameCommand, IValidationResult>
    {
        private readonly IEntityRepository<Game> _gameRepository;
        private readonly IPublisher _eventPublisher;
        private readonly IGameSettings _gameSettings;
        private readonly IRandomizer _randomizer;

        public StartGameCommandHandler(
            IEntityRepository<Game> gameRepository,
            IPublisher eventPublisher,
            IGameSettings gameSettings,
            IRandomizer randomizer)
        {
            _gameRepository = gameRepository;
            _eventPublisher = eventPublisher;
            _gameSettings = gameSettings;
            _randomizer = randomizer;
        }

        public async Task<IValidationResult> Handle(StartGameCommand request, CancellationToken cancellationToken)
        {
            var gameValidation = await ValidateAsync(request.GameId, cancellationToken);
            if (!gameValidation.IsSuccess)
            {
                return gameValidation;
            }

            var game = gameValidation.Value with { CurrentTurn = _randomizer.GetNext(2) };
            await _gameRepository.UpdateAsync(game, cancellationToken);
            await _gameRepository.SaveChangesAsync();

            _ = _eventPublisher.Publish(new PlayerTurnEvent { GameId = game.Id, PlayerId = game.Players[game.CurrentTurn].Id });

            return ValidationResult.Success;
        }

        private async Task<IValidationResult<Game>> ValidateAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
            if (game is null)
            {
                return new ValidationResult<Game>($"Could not found game with an id {gameId}");
            }

            var validationErrors = new List<string>();
            if (game.Players.Any(p => p.IsEmpty()))
            {
                validationErrors.Add("Not all Players have joined the game");
            }
            if (game.Battlefields.Any(b => b.WarshipsPlacement.Count != _gameSettings.BattlefieldWarships.Count))
            {
                validationErrors.Add("Insufficient number of warships placed");
            }
            return validationErrors.Count > 0
                ? new ValidationResult<Game>(validationErrors.ToArray())
                : new ValidationResult<Game>(game);
        }
    }
}
