using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Application.Abstractions.Randomization;
using BattleshipGame.Core.Application.Abstractions.Settings;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Application.ViewModels.Factories;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.PlaceWarship
{
    public class PlaceWarshipCommandHandler : IRequestHandler<PlaceWarshipCommand, IValidationResult<PlayerGameViewModel>>
    {
        private readonly IEntityRepository<Game> _gameRepository;
        private readonly IPublisher _eventPublisher;
        private readonly IGameSettings _gameSettings;
        private readonly IWarshipPlacementValidation _warshipPlacementBehavior;
        private readonly IWarshipPlacementRandomizer _warshipPlacementRandomizer;
        private readonly IPlayerGameViewModelFactory _playerGameViewModelFactory;

        public PlaceWarshipCommandHandler(
            IEntityRepository<Game> gameRepository,
            IPublisher eventPublisher,
            IGameSettings gameSettings,
            IWarshipPlacementValidation warshipPlacementBehavior,
            IWarshipPlacementRandomizer warshipPlacementRandomizer,
            IPlayerGameViewModelFactory playerGameViewModelFactory)
        {
            _gameRepository = gameRepository;
            _eventPublisher = eventPublisher;
            _gameSettings = gameSettings;
            _warshipPlacementBehavior = warshipPlacementBehavior;
            _warshipPlacementRandomizer = warshipPlacementRandomizer;
            _playerGameViewModelFactory = playerGameViewModelFactory;
        }

        public async Task<IValidationResult<PlayerGameViewModel>> Handle(PlaceWarshipCommand request, CancellationToken cancellationToken)
        {
            var placementValidation = await ValidateAsync(request, cancellationToken);
            if (!placementValidation.IsSuccess)
            {
                return new ValidationResult<PlayerGameViewModel>(placementValidation.GetValidationErrors());
            }
            var (game, playerIndex, warshipPlacement) = placementValidation.Value;

            var warshipsPlacement = game.Battlefields[playerIndex].WarshipsPlacement.Add(warshipPlacement);
            game.Battlefields[playerIndex] = game.Battlefields[playerIndex] with { WarshipsPlacement = warshipsPlacement };
            await _gameRepository.UpdateAsync(game, cancellationToken);
            await _gameRepository.SaveChangesAsync();

            if (game.Battlefields[playerIndex].WarshipsPlacement.Count == _gameSettings.BattlefieldWarships.Count)
            {
                _ = _eventPublisher.Publish(new PlayerReadyEvent { GameId = request.GameId, PlayerId = request.PlayerId });
            }

            return new ValidationResult<PlayerGameViewModel>(_playerGameViewModelFactory.Create(game, playerIndex));
        }

        private async Task<IValidationResult<(Game Game, int PlayerIndex, WarshipPlacement WarshipPlacement)>> ValidateAsync(
           PlaceWarshipCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
            if (game is null)
            {
                return new ValidationResult<(Game, int, WarshipPlacement)>($"Could not found game with an id: {request.GameId}");
            }

            var playerIndex = Array.FindIndex(game.Players, p => p.Id == request.PlayerId);
            if (playerIndex == -1)
            {
                return new ValidationResult<(Game, int, WarshipPlacement)>($"Player {request.PlayerId} is not to the game {request.GameId}");
            }

            if (!_warshipPlacementBehavior.CanAddWarship(game.Battlefields[playerIndex], request.Warship))
            {
                return new ValidationResult<(Game, int, WarshipPlacement)>($"{request.Warship.Name} cannot be added to the game {request.GameId}");
            }

            var placement = request.Placement ?? _warshipPlacementRandomizer.GetPlacement(game.Battlefields[playerIndex], request.Warship);
            var warshipPlacement = _warshipPlacementBehavior.TryPlaceWarship(game.Battlefields[playerIndex], request.Warship, placement);
            if (warshipPlacement.HasValue)
            {
                return new ValidationResult<(Game, int, WarshipPlacement)>((game, playerIndex, warshipPlacement.Value));
            }
            else
            {
                return new ValidationResult<(Game, int, WarshipPlacement)>($"Could not place a {request.Warship.Name} to the desired position");
            }
        }
    }
}
