namespace Common
{
    public readonly record struct Result
    {
        private Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
    }

    public readonly record struct Result<TValue>
    {
        private readonly TValue? _value;

        private Result(TValue? value, bool isSuccess, Error error)
        {
            _value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot read the value of a failed result.");

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
        public static Result<TValue> Failure(Error error) => new(default, false, error);
#pragma warning restore CA1000 // Do not declare static members on generic types

#pragma warning disable CA2225 // Operator overloads have named alternates
        public static implicit operator Result<TValue>(TValue value) => Success(value);
        public static implicit operator Result<TValue>(Error error) => Failure(error);
#pragma warning restore CA2225 // Operator overloads have named alternates

        public TOut Match<TOut>(Func<TValue, TOut> onSuccess, Func<Error, TOut> onFailure) =>
            IsSuccess ? onSuccess(_value!) : onFailure(Error);
    }

}
