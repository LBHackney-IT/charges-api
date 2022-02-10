using ChargesApi.V1.Boundary.Response;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services.Interfaces
{
    public interface IAwsS3FileService
    {
        Task<FileLocationResponse> UploadFile(IFormFile formFile, string fileName);
    }
}