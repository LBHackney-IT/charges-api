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
        private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";

        public AddUseCaseTests()
        {
            _mockChargeGateway = new Mock<IChargesApiGateway>();
            _addUseCase = new AddUseCase(_mockChargeGateway.Object);
        }

        [Fact]
        public async Task AddValidModel()
        {
            AddChargeRequest charge = new AddChargeRequest();

            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>()))
                .Returns(Task.CompletedTask);

            await _addUseCase.ExecuteAsync(charge, Token).ConfigureAwait(false);

            _mockChargeGateway.Verify(_ => _.AddAsync(It.IsAny<Charge>()), Times.Once);
        }

        [Fact]
        public async Task AddInvalidModel()
        {
            _mockChargeGateway.Setup(_ => _.AddAsync(It.IsAny<Charge>()))
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
