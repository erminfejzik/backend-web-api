using Application.Abstractions.Authentication;
using Application.Abstractions.Services;
using Application.Repositories.Users;
using Common;
using Infrastructure.Authentication;
using Infrastructure.Database;
using Infrastructure.Services;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddEmailService(configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            #region Repositories
            services.AddScoped<IUserRepository, Repositories.Users.UserRepository>();
            #endregion Repositories
            return services;
        }

        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<SmtpOptions>()
                .Bind(config.GetSection(SmtpOptions.SectionName))
                .ValidateOnStart();

            services.AddTransient<IEmailService, EmailService>();
            return services;
        }
    }
}
