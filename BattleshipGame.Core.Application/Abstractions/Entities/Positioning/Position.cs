namespace BattleshipGame.Core.Application.Abstractions.Entities.Positioning
{
    public readonly struct Position
    {
        public int Left { get; init; }

        public int Top { get; init; }

        public override string ToString()
        {
            return $"{(char)('A' + Left)}{1 + Top}";
        }

        public static bool TryParse(string value, out Position position, int fieldSize)
        {
            position = new Position
            {
                Left = Char.ToUpper(value[0]) - 'A',
                Top = int.TryParse(value[1..], out var top) ? top - 1 : -1
            };
            return position.Left >= 0 && position.Left < fieldSize && position.Top >= 0 && position.Top < fieldSize;
        }

        public static Position FromIndex(int index, int fieldSize) => new Position { Left = index % fieldSize, Top = index / fieldSize };

        public int GetIndexPosition(int fieldSize) => Left + Top * fieldSize;
    }
}
