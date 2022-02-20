using System;

namespace ChargesApi.V1.Boundary.Response
{
    public class FileLocationResponse
    {
        public string RelativePath { get; set; }
        public string BucketName { get; set; }
        public Uri FileUrl { get; set; }
        public int StepNumber { get; set; }
        public int WriteIndex { get; set; }
    }
}
