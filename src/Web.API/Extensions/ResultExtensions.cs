using Common;

namespace Web.API.Extensions
{
    /// <summary>
    /// Map a Result to an HTTP response. This is the single place where an application Error becomes an HTTP response.
    /// </summary>
    public static class ResultExtensions
    {
        public static IResult ToProblem<T>(this Result<T> result) =>
            ToProblem(result);

        public static IResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Cannot convert a successful result to a problem.");

            var error = result.Error;
            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError,
            };

            return Results.Problem(detail: error.Description, title: error.Code, statusCode: status);
        }

        public static IResult Match<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
            result.IsSuccess ? onSuccess(result.Value) : result.ToProblem();
    }
}
