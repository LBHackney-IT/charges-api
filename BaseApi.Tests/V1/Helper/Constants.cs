using ChargeApi.V1.Domain;
using System;

namespace ChargeApi.Tests.V1.Helper
{
    public static class Constants
    {
        public static Guid ID { get; } = Guid.NewGuid();
        public static Guid TARGETID { get; } = Guid.NewGuid();
        public const string TYPE = "Service";
        public const string SUBTYPE = "Water";
        public const string FREQUENCY = "Weekly";
        public const TargetType TARGETTYPE = TargetType.Asset;
        public const decimal AMOUNT = 125;
        public const string STARTDATE = "2021-05-22";
        public const string ENDDATE = "2021-06-22";

        public static Charge ConstructChargeFromConstants()
        {
            var entity = new Charge
            {
                Id = ID,
                TargetId = TARGETID,
                TargetType = TARGETTYPE,
                DetailedCharges = new[]
            {
                new DetailedCharges
                {
                    Amount = AMOUNT,
                    StartDate = DateTime.Parse(STARTDATE),
                    EndDate = DateTime.Parse(ENDDATE),
                    Frequency = FREQUENCY,
                    SubType = SUBTYPE,
                    Type = TYPE
                }
            }
            };
            return entity;
        }

        public static DetailedCharges ConstructDetailedChargesFromConstants()
        {
            var entity = new DetailedCharges
            {
                Amount = AMOUNT,
                EndDate = DateTime.Parse(ENDDATE),
                Frequency = FREQUENCY,
                StartDate = DateTime.Parse(STARTDATE),
                SubType = SUBTYPE,
                Type = TYPE
            };
            return entity;
        }
    }
}
