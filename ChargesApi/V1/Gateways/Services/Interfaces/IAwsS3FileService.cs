using Amazon.S3.Model;
using ChargesApi.V1.Boundary.Response;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services.Interfaces
{
    public interface IAwsS3FileService
    {
        Task<FileLocationResponse> UploadFile(IFormFile formFile, string fileName, IList<Tag> fileTags = null);
        Task<List<FileProcessingLogResponse>> GetProcessedFiles();
        Task<Stream> GetFile(string key);
        Task<FileLocationResponse> UploadPrintRoomFile(IFormFile formFile, string fileName);
    }
}
