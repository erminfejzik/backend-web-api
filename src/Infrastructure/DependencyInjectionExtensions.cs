using Application.Abstractions.Authentication;
using Application.Repositories.Users;
using Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            #region Repositories
            services.AddScoped<IUserRepository, Repositories.Users.UserRepository>();
            #endregion Repositories
            return services;
        }
    }
}
