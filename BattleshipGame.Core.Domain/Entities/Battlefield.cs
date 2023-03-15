using System.Collections.Immutable;

namespace BattleshipGame.Core.Domain.Entities
{
    public sealed record Battlefield
    {
        public Battlefield(int size)
        {
            Size = size;
            ShotsMap = new bool[size * size].ToImmutableArray();
            WarshipsPlacement = ImmutableList<WarshipPlacement>.Empty;
        }

        public int Size { get; }

        public IImmutableList<bool> ShotsMap { get; init; }

        public IImmutableList<WarshipPlacement> WarshipsPlacement { get; init; }
    }
}
