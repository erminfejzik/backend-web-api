using Common;
using FluentValidation.Results;

namespace Application.Extensions
{
    public static class ValidationExtensions
    {
        public static Result<T> ToResultFailure<T>(this ValidationResult validationResult)
        {
            var errors = validationResult.Errors
                .Select(failure => Error.Validation(
                    failure.PropertyName,
                    failure.ErrorMessage))
                .ToList();

            return Result<T>.Failure(errors);
        }
    }
}
