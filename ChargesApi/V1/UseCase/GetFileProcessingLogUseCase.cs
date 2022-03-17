using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.UseCase
{
    public class GetFileProcessingLogUseCase : IGetFileProcessingLogUseCase
    {
        private readonly IAwsS3FileService _s3FileService;

        public GetFileProcessingLogUseCase(IAwsS3FileService s3FileService)
        {
            _s3FileService = s3FileService;
        }

        public async Task<List<FileProcessingLogResponse>> ExecuteAsync(FileType fileType)
        {
            var response = await _s3FileService.GetProcessedFiles(fileType).ConfigureAwait(false);
            return response;
        }
    }
}
