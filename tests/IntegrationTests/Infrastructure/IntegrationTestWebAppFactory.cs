using Application.Abstractions.Services;
using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.MsSql;

namespace IntegrationTests.Infrastructure
{
    public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
            .Build();
        
        public string ConnectionString => _dbContainer.GetConnectionString();
        
        // Mocks
        public Mock<IEmailService> EmailService { get; } = new();
        
        public async ValueTask InitializeAsync()
        {
            await _dbContainer.StartAsync();

            using IServiceScope scope = Services.CreateScope();
            ApplicationDbContext ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await ctx.Database.MigrateAsync();
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(ConnectionString));

                services.RemoveAll<IEmailService>();
                services.AddTransient<IEmailService>(_ => EmailService.Object);
            });
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}