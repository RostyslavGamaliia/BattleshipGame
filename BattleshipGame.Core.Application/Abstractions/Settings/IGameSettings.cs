using BattleshipGame.Core.Application.Abstractions.Entities.Warships;

namespace BattleshipGame.Core.Application.Abstractions.Settings
{
    public interface IGameSettings
    {
        int BattlefieldSize { get; }

        bool WarshipsCanTouch { get; }

        IReadOnlyList<Warship> BattlefieldWarships { get; }
    }
}
