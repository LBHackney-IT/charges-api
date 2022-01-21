using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class AddUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargeGateway;
        private readonly AddUseCase _addUseCase;

        public AddUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargesApiGateway>();
            _addUseCase = new AddUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task AddValidModel()
        {
            AddChargeRequest charge = new AddChargeRequest();

            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _addUseCase.ExecuteAsync(charge, string.Empty).ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.AddAsync(It.IsAny<Charge>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddInvalidModel()
        {
            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            try
            {
                await _addUseCase.ExecuteAsync(null, string.Empty)
                    .ConfigureAwait(false);

                Assert.True(false, "ArgumentNullException should be thrown!");
            }
            catch (Exception ex)
            {
                ex.Should().NotBeNull();
                ex.Should().BeOfType<ArgumentNullException>();
                ex.Message.Should().Be("Value cannot be null. (Parameter 'charge')");
            }
        }
    }
}
