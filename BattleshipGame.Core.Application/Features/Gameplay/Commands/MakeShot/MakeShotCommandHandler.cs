using BattleshipGame.Core.Application.Abstractions.Events;
using BattleshipGame.Core.Application.Abstractions.Persistence;
using BattleshipGame.Core.Application.Abstractions.Validation;
using BattleshipGame.Core.Application.Internals.Validation;
using BattleshipGame.Core.Application.ViewModels;
using BattleshipGame.Core.Application.ViewModels.Factories;
using BattleshipGame.Core.Domain.Entities;
using MediatR;
using System.Collections.Immutable;

namespace BattleshipGame.Core.Application.Features.Gameplay.Commands.MakeShot
{
    public class MakeShotCommandHandler : IRequestHandler<MakeShotCommand, IValidationResult<PlayerGameViewModel>>
    {
        private readonly IEntityRepository<Game> _gameRepository;
        private readonly IPublisher _eventPublisher;
        private readonly IPlayerGameViewModelFactory _playerGameViewModelFactory;

        public MakeShotCommandHandler(
            IEntityRepository<Game> gameRepository,
            IPublisher eventPublisher,
            IPlayerGameViewModelFactory playerGameViewModelFactory)
        {
            _gameRepository = gameRepository;
            _eventPublisher = eventPublisher;
            _playerGameViewModelFactory = playerGameViewModelFactory;
        }

        public async Task<IValidationResult<PlayerGameViewModel>> Handle(MakeShotCommand request, CancellationToken cancellationToken)
        {
            var gameValidation = await ValidateAsync(request, cancellationToken);
            if (!gameValidation.IsSuccess)
            {
                return new ValidationResult<PlayerGameViewModel>(gameValidation.GetValidationErrors());
            }
            var (game, playerIndex) = gameValidation.Value;

            var shotsMap = game.Battlefields[1 - playerIndex].ShotsMap.ToArray();
            var shotIndex = request.Point.GetIndexPosition(game.BattlefieldSize);            
            if (!shotsMap[shotIndex])
            {
                shotsMap[shotIndex] = true;
                game.Battlefields[1 - playerIndex] = game.Battlefields[1 - playerIndex] with { ShotsMap = shotsMap.ToImmutableArray() };
            }
            game = game with { CurrentTurn = game.CurrentTurn + 1 };
            await _gameRepository.UpdateAsync(game, cancellationToken);
            await _gameRepository.SaveChangesAsync();

            var opponentDefeated = game.Battlefields[1 - playerIndex].WarshipsPlacement
                .SelectMany(x => x.GetAllIndexes(game.BattlefieldSize))
                .All(index => shotsMap[index]);
            if (opponentDefeated)
            {
                _ = _eventPublisher.Publish(new GameEndedEvent { GameId = game.Id, Winner = game.Players[playerIndex] });
            }
            else
            {
                _ = _eventPublisher.Publish(new PlayerTurnEvent { GameId = game.Id, PlayerId = game.Players[1 - playerIndex].Id });
            }

            return new ValidationResult<PlayerGameViewModel>(_playerGameViewModelFactory.Create(game, playerIndex));
        }

        private async Task<IValidationResult<(Game Game, int PlayerIndex)>> ValidateAsync(
            MakeShotCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetByIdAsync(request.GameId, cancellationToken);
            if (game is null)
            {
                return new ValidationResult<(Game Game, int PlayerIndex)>($"Could not found game with an id {request.GameId}");
            }
            var playerIndex = Array.FindIndex(game.Players, p => p.Id == request.Player.Id);
            if (game.CurrentTurn % 2 != playerIndex)
            {
                return new ValidationResult<(Game Game, int PlayerIndex)>($"It is not players turn: {request.Player.Name}");
            }
            return new ValidationResult<(Game Game, int PlayerIndex)>((game, playerIndex));
        }
    }
}
