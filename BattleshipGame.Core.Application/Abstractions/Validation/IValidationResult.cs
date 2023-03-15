namespace BattleshipGame.Core.Application.Abstractions.Validation
{
    public interface IValidationResult
    {
        bool IsSuccess { get; }

        string[] GetValidationErrors();
    }

    public interface IValidationResult<T> : IValidationResult where T : notnull
    {
        T Value { get; }
    }
}
