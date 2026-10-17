using Domain.Outbox;
using Domain.Users;
using FluentAssertions;
using IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Web.API.Endpoints.Users;

namespace IntegrationTests.Users
{
    public class UserApiIntegrationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
    {
        private const string ValidEmail = "user@test.com";
        private const string ValidPassword = "password";
        
        [Fact]
        public async Task Register_WithValidData_ReturnsCreatedCode()
        {
            // Arrange
            CancellationToken ct = TestContext.Current.CancellationToken;
            RegisterUserRequest request = new(ValidEmail, ValidPassword);

            // Act 
            HttpResponseMessage response = await Client.PostAsJsonAsync("/users/register", request, ct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
        
        [Fact]
        public async Task Register_WithValidData_PersistsUserAndOutboxMessage()
        {
            // Arrange
            CancellationToken ct = TestContext.Current.CancellationToken;
            
            RegisterUserRequest request = new(ValidEmail, ValidPassword);

            // Act 
            HttpResponseMessage response = await Client.PostAsJsonAsync("/users/register", request, ct);

            // Assert
            JsonElement createdUser = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            Guid userId = createdUser.GetProperty("id").GetGuid();

            User? user = await WithDbAsync<User?>(db => db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct));
            
            List<OutboxMessage> outboxMessages = await WithDbAsync<List<OutboxMessage>>(
                db => db.OutboxMessages.ToListAsync(ct));
            
            VerificationEmailPayload? payload = 
                JsonSerializer.Deserialize<VerificationEmailPayload>(outboxMessages[0].Payload);
            
            user.Should().NotBeNull();
            outboxMessages.Should().HaveCount(1);
            payload.Should().NotBeNull();
            payload.UserId.Should().Be(userId);
        }
        
        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsConflict()
        {
            // Arrange
            CancellationToken ct = TestContext.Current.CancellationToken;
            
            RegisterUserRequest request = new(ValidEmail, ValidPassword);
            await Client.PostAsJsonAsync("/users/register", request, ct);

            // Act 
            HttpResponseMessage response = await Client.PostAsJsonAsync("/users/register", request, ct);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}