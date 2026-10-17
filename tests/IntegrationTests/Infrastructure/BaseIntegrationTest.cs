using Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Respawn;

namespace IntegrationTests.Infrastructure
{
    [CollectionDefinition("ApiIntegrationTests")]
    public class CollectionApi : ICollectionFixture<IntegrationTestWebAppFactory>;
    
    [Collection("ApiIntegrationTests")]
    public abstract class BaseIntegrationTest(IntegrationTestWebAppFactory factory) : IAsyncLifetime
    {
        private static Respawner? _respawner;
        
        private IntegrationTestWebAppFactory Factory { get; } = factory;
        protected HttpClient Client { get; } = factory.CreateClient();

        public async ValueTask InitializeAsync()
        {
            Factory.EmailService.Reset();

            await using SqlConnection conn = new (Factory.ConnectionString);
            await conn.OpenAsync(TestContext.Current.CancellationToken);

            _respawner ??= await Respawner.CreateAsync(conn, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = ["__EFMigrationsHistory"]
            });
            await _respawner.ResetAsync(conn);
        }
        
        protected async Task<T> WithDbAsync<T>(Func<ApplicationDbContext, Task<T>> action)
        {
            using IServiceScope scope = Factory.Services.CreateScope();
            return await action(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
        }
        
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}