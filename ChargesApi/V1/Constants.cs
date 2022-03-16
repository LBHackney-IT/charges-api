using System;

namespace ChargesApi.V1
{
    public static class Constants
    {
        public const string CorrelationId = "x-correlation-id";
        public const string ServiceChargeType = "Service";
        public const string ChargeTableName = "Charges";
        public const string EstimateTypeFile = "Estimate";
        public const string ActualTypeFile = "Actual";
        public const string AttributeNotExistId = "attribute_not_exists(id)";
        public const int PageSize = 8000;
        public const int Page = 1;
        public const int PerBatchProcessingCount = 10;
        public const string UtcDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
        public const string EstimateUpload = "uploads/";
        public const string PrintRentRoom = "printoutput/";

    }
}
