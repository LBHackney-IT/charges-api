using System;

namespace ChargesApi.V1.Boundary.Response
{
    public class FileProcessingLogResponse
    {
        public string FileName { get; set; }
        public string FileStatus { get; set; }
        public Uri FileUrl { get; set; }
        public DateTimeOffset DateUploaded { get; set; }
        public string Year { get; set; }
        public string ValuesType { get; set; }
        public string FileType { get; set; }
    }
}
