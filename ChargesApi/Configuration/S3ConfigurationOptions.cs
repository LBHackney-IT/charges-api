namespace ChargesApi.Configuration
{
    public class S3ConfigurationOptions
    {
        public const string SectionName = "S3Configuration";
        public string BucketName { get; set; }
    }
}
