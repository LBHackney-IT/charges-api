using AutoFixture;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.UseCase;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Factories;

namespace ChargesApi.Tests.V1.UseCase
{
    public class AddEstimateChargesUseCaseTests
    {
        private readonly Mock<IChargesApiGateway> _mockChargeGateway;
        private readonly Mock<IHousingSearchService> _mockHousingSearchService;
        private readonly Mock<IFinancialSummaryService> _mockFinancialService;
        private readonly Mock<IAwsS3FileService> _mockS3FileService;
        private readonly Mock<ISnsGateway> _mockSnsGateway;
        private readonly Mock<ISnsFactory> _mockSnsFactory;
        private readonly Mock<ILogger<AddEstimateChargesUseCase>> _mockLogger;

        //private const string Token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJ0ZXN0IiwiaWF0IjoxNjM5NDIyNzE4LCJleHAiOjE5ODY1Nzc5MTgsImF1ZCI6InRlc3QiLCJzdWIiOiJ0ZXN0IiwiZ3JvdXBzIjpbInNvbWUtdmFsaWQtZ29vZ2xlLWdyb3VwIiwic29tZS1vdGhlci12YWxpZC1nb29nbGUtZ3JvdXAiXSwibmFtZSI6InRlc3RpbmcifQ.IcpQ00PGVgksXkR_HFqWOakgbQ_PwW9dTVQu4w77tmU";
        private readonly Fixture _fixture;

        private readonly AddEstimateChargesUseCase _addEstimateChargesUseCase;

        public AddEstimateChargesUseCaseTests()
        {
            _fixture = new Fixture();
            _mockChargeGateway = new Mock<IChargesApiGateway>();
            _mockHousingSearchService = new Mock<IHousingSearchService>();
            _mockLogger = new Mock<ILogger<AddEstimateChargesUseCase>>();
            _mockFinancialService = new Mock<IFinancialSummaryService>();
            _mockS3FileService = new Mock<IAwsS3FileService>();
            _mockSnsGateway = new Mock<ISnsGateway>();
            _mockSnsFactory = new Mock<ISnsFactory>();

            _mockS3FileService.Setup(s => s.UploadFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync(new FileLocationResponse
                {
                    RelativePath = $"uploads/{Guid.NewGuid()}.xslx",
                    BucketName = "test-bucket",
                    FileUrl = null
                });
            _addEstimateChargesUseCase = new AddEstimateChargesUseCase(_mockChargeGateway.Object,
                _mockHousingSearchService.Object,
                _mockFinancialService.Object,
               _mockLogger.Object,
                _mockS3FileService.Object,
                _mockSnsGateway.Object,
                _mockSnsFactory.Object
                );
        }

        //[Fact]
        //public async Task AddValidExcelFileWithValidTransformWithSuccessChargeSave()
        //{
        //    var assetListResponse = _fixture.Build<AssetListResponse>().Create();
        //    _mockHousingSearchService.Setup(_ => _.GetAssets(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>())).ReturnsAsync(assetListResponse);

        //    _mockChargeGateway.Setup(x => x.AddTransactionBatchAsync(It.IsAny<List<Charge>>())).ReturnsAsync(true);

        //    _mockFinancialService.Setup(_ => _.AddEstimateSummary(It.IsAny<AddAssetSummaryRequest>())).ReturnsAsync(true);
        //    var resultCount = 0;

        //    using var sourceFile = File.OpenRead(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.IndexOf("bin\\"))), "EstimatesTest.xlsx"));
        //    using (var stream = sourceFile)
        //    {
        //        var file = new FormFile(stream, 0, stream.Length, null, "EstimatesTest.xlsx")
        //        {
        //            Headers = new HeaderDictionary(),
        //            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        //        };

        //        resultCount = await _addEstimateChargesUseCase.AddEstimates(file, ChargeGroup.Leaseholders, Token).ConfigureAwait(false);

        //    }
        //    resultCount.Should().BeGreaterThan(0);
        //    _mockChargeGateway.Verify(_ => _.AddTransactionBatchAsync(It.IsAny<List<Charge>>()), Times.Exactly(1));
        //}
    }
}
