using Amazon.S3;
using Amazon.S3.Model;
using ChargesApi.Configuration;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Gateways.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways.Services
{
    public class AwsS3FileService : IAwsS3FileService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3ConfigurationOptions _s3Settings;

        public AwsS3FileService(IAmazonS3 s3Client, IOptions<S3ConfigurationOptions> s3Settings)
        {
            _s3Client = s3Client;
            _s3Settings = s3Settings.Value;
        }

        public async Task<FileLocationResponse> UploadFile(IFormFile formFile, string fileName)
        {
            var location = $"uploads/{fileName}";
            using (var stream = formFile.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    Key = location,
                    BucketName = _s3Settings.BucketName ?? "test bucket",
                    InputStream = stream,
                    AutoCloseStream = true,
                    TagSet = new List<Tag> { new Tag { Key = "status", Value = "Uploaded" } },
                    ContentType = formFile.ContentType
                };
                try
                {
                    await _s3Client.PutObjectAsync(putRequest).ConfigureAwait(false);
                    return new FileLocationResponse
                    {
                        RelativePath = location,
                        BucketName = _s3Settings.BucketName,
                        FileUrl = null
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to upload file to S3", ex.InnerException);
                }
            }
        }
    }
}
