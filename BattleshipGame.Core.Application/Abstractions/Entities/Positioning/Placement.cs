namespace BattleshipGame.Core.Application.Abstractions.Entities.Positioning
{
    public readonly struct Placement
    {
        public Position Position { get; init; }

        public Direction Direction { get; init; }

        public Position GetEndPosition(int length) => Direction == Direction.Vertical
            ? new Position { Left = Position.Left, Top = Position.Top + length - 1 }
            : new Position { Left = Position.Left + length - 1, Top = Position.Top };
    }
}
