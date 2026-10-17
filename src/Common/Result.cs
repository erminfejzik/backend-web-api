namespace Common
{
    public readonly record struct Result
    {
        private Result(bool isSuccess, List<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public List<Error> Errors { get; }

        public static Result Success() => new(true, []);
        public static Result Failure(List<Error> errors) => new(false, errors);
    }

    public readonly record struct Result<TValue>
    {
        private readonly TValue? _value;

        private Result(TValue? value, bool isSuccess, List<Error> errors)
        {
            _value = value;
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public List<Error> Errors { get; }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot read the value of a failed result.");

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static Result<TValue> Success(TValue value) => new(value, true, []);
        public static Result<TValue> Failure(List<Error> errors) => new(default, false, errors);
#pragma warning restore CA1000 // Do not declare static members on generic types

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(List<Error> errors) => Failure(errors);
#pragma warning restore CA2225 // Operator overloads have named alternates

        public TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<List<Error>, TOut> onFailure) =>
            IsSuccess ? onSuccess(_value!) : onFailure(Errors);
    }

}
