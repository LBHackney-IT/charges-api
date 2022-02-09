using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ChargesApi.V1.Gateways.Services.Interfaces
{
    public interface IAwsS3FileService
    {
        Task<string> UploadFile(IFormFile formFile, string fileName);
        Task<Stream> GetFile(string key);
        Task<bool> DeleteFile(string key);
    }
}
