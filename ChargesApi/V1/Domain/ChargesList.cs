using System;

namespace ChargesApi.V1.Domain
{
    public class ChargesList
    {
        public Guid Id { get; set; }
        /// <summary>
        /// ex: GroundMaintenance
        /// </summary>
        public string ChargeName { get; set; }
        /// <summary>
        /// ex: DCB
        /// </summary>
        public string ChargeCode { get; set; }
        /// <summary>
        ///  ex: Estate, Block, Property
        /// </summary>
        public ChargeType ChargeType { get; set; }
        /// <summary>
        ///  ex: Tenant / LH / other
        /// </summary>
        public ChargeGroup ChargeGroup { get; set; }
    }
}
