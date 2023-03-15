namespace BattleshipGame.Core.Application.Abstractions.Entities.Warships
{
    public abstract class Warship
    {
        public string Name { get; init; } = string.Empty;

        public abstract int Length { get; }
    }
}
