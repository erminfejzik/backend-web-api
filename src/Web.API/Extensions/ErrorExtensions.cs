using Common;

namespace Web.API.Extensions
{
    /// <summary>
    /// Map a Error to an HTTP response. This is the single place where an application Error becomes an HTTP response.
    /// </summary>
    public static class ErrorExtensions
    {
        public static IResult ToProblem(this List<Error> errors)
        {
            if (errors.Count == 0)
            {
                throw new InvalidOperationException("Cannot convert an empty list of errors to a problem.");
            }

            var error = errors[0];

            var status = error.Type switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };

            string detail = errors.Count > 1
                ? "Multiple errors occurred. See the 'errors' property for details."
                : error.Description;

            var extensionsValue = errors.GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return Results.Problem(
                detail: detail,
                statusCode: status,
                extensions: [new KeyValuePair<string, object?>("errors", extensionsValue)]
            );
        }
    }
}
