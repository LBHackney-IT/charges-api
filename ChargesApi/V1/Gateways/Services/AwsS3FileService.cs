using Amazon.S3;
using Amazon.S3.Model;
using ChargesApi.Configuration;
using ChargesApi.V1.Boundary.Response;
using ChargesApi.V1.Gateways.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public async Task<FileLocationResponse> UploadFile(IFormFile formFile, string fileName, IList<Tag> fileTags = null)
        {
            var location = $"uploads/{fileName}";
            using (var stream = formFile.OpenReadStream())
            {
                var tagSet = new List<Tag> { new Tag { Key = "status", Value = "Uploaded" } };
                if (fileTags != null && fileTags.Count > 0)
                {
                    tagSet.AddRange(fileTags);
                }
                var putRequest = new PutObjectRequest
                {
                    Key = location,
                    BucketName = _s3Settings.BucketName ?? "test bucket",
                    InputStream = stream,
                    AutoCloseStream = true,
                    TagSet = tagSet,
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
                    throw new Exception($"Failed to upload file to S3  {ex.Message}", ex.InnerException);
                }
            }
        }

        public async Task<List<FileProcessingLogResponse>> GetProcessedFiles()
        {
            var prefix = "uploads/";

            var request = new ListObjectsV2Request()
            {
                BucketName = _s3Settings.BucketName,
                Prefix = prefix
            };
            var listObjectResponse = await _s3Client.ListObjectsV2Async(request).ConfigureAwait(false);

            var s3ObjectList = listObjectResponse.S3Objects.OrderByDescending(o => o.LastModified).Take(20).ToList();

            var filesList = new List<FileProcessingLogResponse>();

            foreach (var s3Object in s3ObjectList)
            {
                var (year, fileStatus, valuesType) = await GetObjectTags(s3Object.Key).ConfigureAwait(false);
                var fileUrl = GeneratePreSignedUrl(s3Object.Key);
                filesList.Add(new FileProcessingLogResponse
                {
                    FileName = Path.GetFileNameWithoutExtension(s3Object.Key),
                    FileStatus = fileStatus,
                    FileUrl = new Uri(fileUrl),
                    DateUploaded = s3Object.LastModified,
                    Year = year,
                    ValuesType = valuesType
                });
            }

            return filesList;
        }

        private string GeneratePreSignedUrl(string objectKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            string url = _s3Client.GetPreSignedURL(request);
            return url;
        }

        private async Task<(string year, string fileStatus, string valuesType)> GetObjectTags(string objectKey)
        {
            var request = new GetObjectTaggingRequest
            {
                BucketName = _s3Settings.BucketName,
                Key = objectKey
            };

            var taggingResponse = await _s3Client.GetObjectTaggingAsync(request).ConfigureAwait(false);
            var year = taggingResponse.Tagging.Where(t => t.Key == "year").Select(t => t.Value).FirstOrDefault();
            var fileStatus = taggingResponse.Tagging.Where(t => t.Key == "status").Select(t => t.Value).FirstOrDefault();
            var valuesType = taggingResponse.Tagging.Where(t => t.Key == "valuesType").Select(t => t.Value).FirstOrDefault();
            return (year, fileStatus, valuesType);
        }
    }
}
