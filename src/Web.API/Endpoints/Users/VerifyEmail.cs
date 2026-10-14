using Application.Features.Users;
using Carter;
using Common;
using MediatR;
using Web.API.Extensions;

namespace Web.API.Endpoints.Users
{
    public sealed class VerifyEmail : ICarterModule
    {
        sealed record Request(string Token);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/verify-email", async (Request request, ISender sender, CancellationToken ct) =>
            {
                Result result = await sender.Send(new VerifyEmailCommand(request.Token), ct);

                return result.IsSuccess
                    ? Results.Ok(new { Message = "Email address verified successfully." })
                    : result.Errors.ToProblem();
            })
                .WithName("VerifyEmail")
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
