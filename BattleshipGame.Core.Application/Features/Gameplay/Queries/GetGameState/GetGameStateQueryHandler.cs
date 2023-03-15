using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Application.ViewModels.Factories;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.Gameplay.Queries.GetGameState
{
    public class GetGameStateQueryHandler : IRequestHandler<GetGameStateQuery, IValidationResult<PlayerGameViewModel>>
    {
        private readonly IEntityRepository<Game> _gameRepository;
        private readonly IPlayerGameViewModelFactory _playerGameViewModelFactory;

        public GetGameStateQueryHandler(
            IEntityRepository<Game> gameRepository,
            IPlayerGameViewModelFactory playerGameViewModelFactory)
        {
            _gameRepository = gameRepository;
            _playerGameViewModelFactory = playerGameViewModelFactory;
        }

        public async Task<IValidationResult<PlayerGameViewModel>> Handle(GetGameStateQuery request, CancellationToken cancellationToken)
        {
            var gameValidation = await ValidateAsync(request, cancellationToken);
            return gameValidation.IsSuccess
                ? new ValidationResult<PlayerGameViewModel>(
                    _playerGameViewModelFactory.Create(gameValidation.Value.Game, gameValidation.Value.PlayerIndex))
                : new ValidationResult<PlayerGameViewModel>(gameValidation.GetValidationErrors());
        }

        private async Task<IValidationResult<(Game Game, int PlayerIndex)>> ValidateAsync(
            GetGameStateQuery request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
            if (game is null)
            {
                return new ValidationResult<(Game Game, int PlayerIndex)>($"Could not found game with an id: {request.GameId}");
            }
            var playerIndex = Array.FindIndex(game.Players, p => p.Id == request.PlayerId);
            if (playerIndex == -1)
            {
                return new ValidationResult<(Game Game, int PlayerIndex)>($"Player {request.PlayerId} is not to the game {request.GameId}");
            }
            return new ValidationResult<(Game Game, int PlayerIndex)>((game, playerIndex));
        }
    }
}
