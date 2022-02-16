using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChargesApi.Tests.V1.UseCase
{
    public class UpdateUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargeGateway;
        private readonly UpdateUseCase _updateUseCase;
        private readonly Mock<ISnsGateway> _snsGateway;
        private readonly Mock<ISnsFactory> _snsFactory;
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";

        public UpdateUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargesApiGateway>();
            _snsGateway = new Mock<ISnsGateway>();
            _snsFactory = new Mock<ISnsFactory>();
            _updateUseCase = new UpdateUseCase(_mockChargeGateway.Object,
                                               _snsGateway.Object,
                                               _snsFactory.Object);
        }

        [Fact]
        public async Task UpdateValidModel()
        {
            _mockChargeGateway.Setup(_ => _.UpdateAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            await _updateUseCase.ExecuteAsync(new ChargeResponse(), Token)
                .ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.UpdateAsync(It.IsAny<Charge>()), Times.Once);
        }

        [Fact]
        public async Task UpdateInvalidModel()
        {
            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            try
            {
                await _updateUseCase.ExecuteAsync(null, Token)
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
