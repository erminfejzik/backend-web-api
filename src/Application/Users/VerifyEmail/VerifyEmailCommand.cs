using Common;
using MediatR;

namespace Application.Users.VerifyEmail
{
    public sealed record VerifyEmailCommand(string Token) : IRequest<Result>;
}
