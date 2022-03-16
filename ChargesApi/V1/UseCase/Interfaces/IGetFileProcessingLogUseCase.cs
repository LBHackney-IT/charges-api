using ChargesApi.V1.Boundary.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IGetFileProcessingLogUseCase
    {
        Task<List<FileProcessingLogResponse>> ExecuteAsync(FileType fileType);
    }
}
