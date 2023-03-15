namespace BattleshipGame.Core.Domain.Entities
{
    public sealed record Player
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public bool IsEmpty() => Name == string.Empty && Id == Guid.Empty;
    }
}
