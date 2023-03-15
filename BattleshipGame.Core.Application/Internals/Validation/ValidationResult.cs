using BattleshipGame.Core.Application.Abstractions.Validation;

namespace BattleshipGame.Core.Application.Internals.Validation
{
    internal class ValidationResult : IValidationResult
    {
        private readonly string[] _validationErrors;

        public ValidationResult(params string[] validationErrors) : this(validationErrors.Length > 0, validationErrors)
        {
        }

        protected ValidationResult(bool isSuccess, params string[] validationErrors)
        {
            IsSuccess = isSuccess;
            _validationErrors = validationErrors;
        }

        public bool IsSuccess { get; }

        public string[] GetValidationErrors() => _validationErrors;

        public static IValidationResult Success { get; } = new ValidationResult(true);
    }

    internal class ValidationResult<T> : ValidationResult, IValidationResult<T> where T : notnull
    {
        public ValidationResult(params string[] validationErrors) : base(false, validationErrors)
        {
        }

        public ValidationResult(T value) : base(true)
        {
            Value = value;
        }

        public T Value { get; } = default!;
    }
}
