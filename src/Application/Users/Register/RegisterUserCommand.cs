using Common;
using MediatR;

namespace Application.Users.Register
{
    public sealed record RegisterUserCommand(string Email, string Password) : IRequest<Result<Guid>>;
}
