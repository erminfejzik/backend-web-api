using Common;
using MediatR;

namespace Application.Features.Users
{
    public sealed record VerifyEmailCommand(string Token) : IRequest<Result>;
}
