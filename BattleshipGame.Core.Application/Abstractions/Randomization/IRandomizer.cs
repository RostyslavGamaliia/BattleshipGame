namespace BattleshipGame.Core.Application.Abstractions.Randomization
{
    public interface IRandomizer
    {
        int GetNext(int maxValue);
    }
}
