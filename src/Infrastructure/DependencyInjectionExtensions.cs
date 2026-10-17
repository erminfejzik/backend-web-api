using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Application.Abstractions.Services;
using Application.Repositories;
using Common;
using Infrastructure.Authentication;
using Infrastructure.BackgroundServices;
using Infrastructure.Database;
using Infrastructure.Repositories;
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
            services.Services(configuration);
            services.AddDatabase(configuration);

            return services;
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            #region Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
            #endregion Repositories
        }

        public static IServiceCollection Services(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddSingleton<IUserTokenGenerator, UserTokenGenerator>();

            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            services.AddOptions<SmtpOptions>()
                .Bind(config.GetSection(SmtpOptions.SectionName))
                .ValidateOnStart();
            services.AddTransient<IEmailService, EmailService>();

            services.AddHostedService<OutboxMessageProcessor>();

            return services;
        }
    }
}
