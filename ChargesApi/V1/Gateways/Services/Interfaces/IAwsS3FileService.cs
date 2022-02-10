using System.IO;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Response;
using Microsoft.AspNetCore.Http;

namespace ChargesApi.V1.Gateways.Services.Interfaces
{
    public interface IAwsS3FileService
    {
        Task<FileLocationResponse> UploadFile(IFormFile formFile, string fileName);
        Task<byte[]> GetFile(string key);
        Task<bool> DeleteFile(string key);
    }
}
