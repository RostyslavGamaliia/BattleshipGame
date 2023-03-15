using BattleshipGame.Core.Application.Abstractions.Entities.Warships;
using BattleshipGame.Core.Application.Abstractions.Settings;
using System.Collections.Immutable;

namespace BattleshipGame.Core.Application.Internals.Settings
{
    internal class GameSettings : IGameSettings
    {
        public int BattlefieldSize => 10;

        public bool WarshipsCanTouch => false;

        public IReadOnlyList<Warship> BattlefieldWarships { get; } = new List<Warship>
        {
            new Battleship(),
            new Destroyer(),
            new Destroyer()
        }.ToImmutableArray();
    }
}
