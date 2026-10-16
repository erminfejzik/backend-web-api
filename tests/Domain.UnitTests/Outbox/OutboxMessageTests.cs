using Domain.Outbox;
using FluentAssertions;

namespace Domain.UnitTests.Outbox
{
    public sealed class OutboxMessageTests
    {
        const string ExamplePayload = "{\"UserId\":\"3c72125d-4955-edb8-70c1-01a144738c8b\"}";

        [Theory]
        [InlineData("InvalidType", "")]
        [InlineData(OutboxMessageTypes.VerificationEmail, "")]
        public void Create_Should_Throw_ArgumentException_WithInvalidData(string type, string payload)
        {
            Action act = () =>
            {
                OutboxMessage.Create(type, payload);
            };

            act.Should().ThrowExactly<ArgumentException>("because of invalid type or payload");
        }

        [Fact]
        public void Create_Should_Succeed_WithValidData()
        {
            var outboxMessage = OutboxMessage.Create(OutboxMessageTypes.VerificationEmail, ExamplePayload);

            outboxMessage.Type.Should().Be(OutboxMessageTypes.VerificationEmail);
            outboxMessage.Payload.Should().Be(ExamplePayload);
        }

        [Fact]
        public void MarkProcessed_Should_SetProcessedAt()
        {
            var outboxMessage = OutboxMessage.Create(OutboxMessageTypes.VerificationEmail, ExamplePayload);

            DateTimeOffset now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            outboxMessage.MarkProcessed(now);

            outboxMessage.ProcessedAt.Should().BeExactly(now);
        }

        [Fact]
        public void RegisterFailure_Should_Return_True_WhenFailure_IsPermanent()
        {
            var outboxMessage = OutboxMessage.Create(OutboxMessageTypes.VerificationEmail, ExamplePayload);

            DateTimeOffset now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            bool result =outboxMessage.RegisterFailure(string.Empty, true, 3, TimeSpan.FromSeconds(30), now);

            result.Should().BeTrue();
            outboxMessage.ProcessedAt.Should().BeExactly(now);
            outboxMessage.Error.Should().Be("Unknown error");
        }

        [Fact]
        public void RegisterFailure_Should_ReturnFalse_WhenFailure_IsNotPermanent()
        {
            var outboxMessage = OutboxMessage.Create(OutboxMessageTypes.VerificationEmail, ExamplePayload);
            var longError = new string('e', 1001);
            DateTimeOffset now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
            TimeSpan baseDelay = TimeSpan.FromSeconds(30);

            bool result = outboxMessage.RegisterFailure(longError, false, 3, baseDelay, now);

            result.Should().BeFalse();
            outboxMessage.NextAttemptAt.Should().BeExactly(now.Add(baseDelay));
            outboxMessage.Error.Should().HaveLength(1000);
        }
    }
}
