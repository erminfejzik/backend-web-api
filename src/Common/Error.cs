namespace Common
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Forbidden = 4
    }

#pragma warning disable CA1716 // Identifiers should not match keywords
    public readonly record struct Error(string Code, string Description, ErrorType Type)
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
        public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
        public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
        public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);
    }
}
