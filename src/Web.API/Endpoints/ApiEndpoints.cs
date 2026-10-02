using Carter;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Endpoints
{
    public class ApiEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", () => System.DateTime.UtcNow.ToString(
                "yyyy-MM-dd HH:mm:ss",
                System.Globalization.CultureInfo.CurrentCulture));

            app.MapGet("/name/{name}", (string name, [FromServices] Serilog.ILogger logger) =>
            {
                logger.Information("Request received for name: {Name}", name);
                return $"Hello, {name}!";
            });
        }
    }
}
