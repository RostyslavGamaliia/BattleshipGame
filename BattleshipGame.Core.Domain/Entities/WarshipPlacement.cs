namespace BattleshipGame.Core.Domain.Entities
{
    public readonly struct WarshipPlacement
    {
        public int StartCellIndex { get; init; }

        public int EndCellIndex { get; init; }

        public IEnumerable<int> GetAllIndexes(int filedSize)
        {
            var startCellIndex = StartCellIndex;
            return EndCellIndex - StartCellIndex >= filedSize
                ? Enumerable.Range(0, (EndCellIndex - StartCellIndex) / filedSize + 1).Select(x => startCellIndex + x * filedSize)
                : Enumerable.Range(0, (EndCellIndex - StartCellIndex) + 1).Select(x => startCellIndex + x);
        }
    }
}
