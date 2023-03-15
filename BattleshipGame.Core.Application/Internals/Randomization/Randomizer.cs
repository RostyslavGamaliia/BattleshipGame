using BattleshipGame.Core.Application.Abstractions.Randomization;

namespace BattleshipGame.Core.Application.Internals.Randomization
{
    internal class Randomizer : IRandomizer
    {
        private readonly Random _rand = new();

        public int GetNext(int maxValue) => _rand.Next(maxValue);
    }
}
