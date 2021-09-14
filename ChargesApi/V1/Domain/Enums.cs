using System.Text.Json.Serialization;

namespace ChargesApi.V1.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TargetType
    {
        asset,
        tenure,
        block,
        estate
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeMaintenanceStatus
    {
        pending,
        applied
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeType
    {
        estate,
        block,
        property
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ChargeGroup
    {
        tenants,
        leaseholders
    }
}
