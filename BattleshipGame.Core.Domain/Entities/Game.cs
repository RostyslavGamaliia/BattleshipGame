namespace BattleshipGame.Core.Domain.Entities
{
    public sealed record Game : IEntity
    {
        public Game()
        {
            Battlefields = new Battlefield[2];
            Players = new[] { new Player(), new Player() };
        }

        public Guid Id { get; init; }

        public int BattlefieldSize { get; init; }

        public int CurrentTurn { get; init; }

        public Battlefield[] Battlefields { get; }

        public Player[] Players { get; }
    }
}
