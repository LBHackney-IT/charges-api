using ChargesApi.V1.Domain;
using System;

namespace ChargesApi.V1.Boundary.Response
{
    public class ChargesListResponse
    {
        public Guid Id { get; set; }
        public string ChargeName { get; set; }
        public string ChargeCode { get; set; }
        public ChargeType ChargeType { get; set; }
        public ChargeGroup ChargeGroup { get; set; }
    }
}
