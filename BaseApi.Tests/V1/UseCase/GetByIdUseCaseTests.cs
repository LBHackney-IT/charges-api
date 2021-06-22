using ChargeApi.V1.Gateways;
using ChargeApi.V1.UseCase;
using Moq;
using Xunit;

namespace ChargeApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests
    {
        private Mock<IChargeApiGateway> _mockGateway;
        private GetByIdUseCase _classUnderTest;

        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IChargeApiGateway>();
            _classUnderTest = new GetByIdUseCase(_mockGateway.Object);
        }

        //TODO: test to check that the use case retrieves the correct record from the database.
        //Guidance on unit testing and example of mocking can be found here https://github.com/LBHackney-IT/lbh-base-api/wiki/Writing-Unit-Tests
    }
}
