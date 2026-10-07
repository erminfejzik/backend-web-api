using Carter;
using Common;
using MediatR;
using Web.API.Extensions;

namespace Web.API.Endpoints.Users
{
    public class Register : ICarterModule
    {
        sealed record RegisterUserRequest(string Email, string Password);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/users/register", async (RegisterUserRequest body, ISender sender, CancellationToken ct) =>
            {
                Result<Guid> result = await sender.Send(new Application.Features.Users.RegisterUserCommand(
                    body.Email,
                    body.Password), ct);

                return result.IsSuccess 
                    ? Results.Created("/users/{id}", new { Id = result.Value.ToString() })
                    : result.Error.ToProblem();
            })
                .WithName("RegisterUser")
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem()
                .ProducesProblem(StatusCodes.Status409Conflict);
        }
    }
}
