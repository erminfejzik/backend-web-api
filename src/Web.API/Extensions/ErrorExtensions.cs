using Common;

namespace Web.API.Extensions
{
    /// <summary>
    /// Map a Error to an HTTP response. This is the single place where an application Error becomes an HTTP response.
    /// </summary>
    public static class ErrorExtensions
    {
        public static IResult ToProblem(this Error error)
        {
            if (error.Type == ErrorType.Failure)
            {
                throw new InvalidOperationException("Cannot convert a empty error to a problem.");
            }

            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

            return Results.Problem(detail: error.Description, title: error.Code, statusCode: status);

        }
    }
}
