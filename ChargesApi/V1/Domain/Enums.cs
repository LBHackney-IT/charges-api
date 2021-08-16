using System.Text.Json.Serialization;

namespace ChargeApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TargetType
    {
        Asset,
        Tenure
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeMaintenanceStatus
    {
        Pending,
        Applied
    }
}