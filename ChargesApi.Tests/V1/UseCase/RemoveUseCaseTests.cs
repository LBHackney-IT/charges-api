using ChargeApi.V1.Domain;
using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargeApi.Tests.V1.UseCase
{
    public class RemoveUseCaseTests
    {
        private readonly Mock<IChargeApiGateway> _mockChargeGateway;
        private readonly RemoveUseCase _removeUseCase;

        public RemoveUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargeApiGateway>();
            _removeUseCase = new RemoveUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task RemoveChargeValidId()
        {
            _mockChargeGateway.Setup(_ => _.GetChargeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Charge());

            _mockChargeGateway.Setup(_ => _.RemoveAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            await _removeUseCase.ExecuteAsync(new Guid("43f50ead-ff80-4ea3-bdd3-6db654c7fc88"))
                .ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.RemoveAsync(It.IsAny<Charge>()), Times.Once);
        }

        [Fact]
        public async Task RemoveChargeInvalidId()
        {
            _mockChargeGateway.Setup(_ => _.GetChargeByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Charge) null);

            _mockChargeGateway.Setup(_ => _.RemoveAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            var guid = Guid.NewGuid();

            try
            {
                await _removeUseCase.ExecuteAsync(guid)
                    .ConfigureAwait(false);

                Assert.True(false, "Exception should be thrown!");
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
                ex.Should().BeOfType<Exception>();
                ex.Message.Should().Be($"Cannot find charge with provided id: {guid}");
            }
        }
    }
}
