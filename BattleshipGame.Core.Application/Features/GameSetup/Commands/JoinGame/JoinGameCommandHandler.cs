using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Application.ViewModels.Factories;
using BattleshipGame.Core.Domain.Entities;
using MediatR;

namespace BattleshipGame.Core.Application.Features.GameSetup.Commands.JoinGame
{
    public class JoinGameCommandHandler : IRequestHandler<JoinGameCommand, IValidationResult<PlayerGameViewModel>>
    {
        private readonly IEntityRepository<Game> _gameRepository;
        private readonly IPlayerGameViewModelFactory _playerGameViewModelFactory;

        public JoinGameCommandHandler(
            IEntityRepository<Game> gameRepository,
            IPlayerGameViewModelFactory playerGameViewModelFactory)
        {
            _gameRepository = gameRepository;
            _playerGameViewModelFactory = playerGameViewModelFactory;
        }

        public async Task<IValidationResult<PlayerGameViewModel>> Handle(JoinGameCommand request, CancellationToken cancellationToken)
        {
            var gameValidation = await ValidateAsync(request, cancellationToken);
            if (!gameValidation.IsSuccess)
            {
                return new ValidationResult<PlayerGameViewModel>(gameValidation.GetValidationErrors());
            }
            var game = gameValidation.Value;
            game.Players[1] = request.Player;
            await _gameRepository.UpdateAsync(game, cancellationToken);
            await _gameRepository.SaveChangesAsync();

            return new ValidationResult<PlayerGameViewModel>(_playerGameViewModelFactory.Create(game, 1));
        }

        private async Task<IValidationResult<Game>> ValidateAsync(
           JoinGameCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
            if (game is null)
            {
                return new ValidationResult<Game>($"Could not found game with an id: {request.GameId}");
            }
            if (!game.Players[1].IsEmpty())
            {
                return new ValidationResult<Game>($"Second player already joined to the game {request.GameId}");
            }
            return new ValidationResult<Game>(game);
        }
    }
}
