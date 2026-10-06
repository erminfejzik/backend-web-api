using Common;
using MediatR;

namespace Application.Features.Users
{
    public sealed record RegisterUserCommand(string Email, string Password) : IRequest<Result<Guid>>;
}
