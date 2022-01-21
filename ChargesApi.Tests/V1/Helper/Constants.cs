using ChargesApi.V1.Domain;
using System;

namespace ChargesApi.Tests.V1.Helper
{
    public static class Constants
    {
        public static Guid ID { get; } = Guid.NewGuid();
        public static Guid TARGETID { get; } = Guid.NewGuid();
        public const string TYPE = "Service";
        public const string SUBTYPE = "Water";
        public const string FREQUENCY = "Weekly";
        public const TargetType TARGETTYPE = TargetType.Dwelling;
        public const decimal AMOUNT = 125;
        public const string STARTDATE = "2021-05-22";
        public const string ENDDATE = "2021-06-22";
        public const string CHARGECODE = "DCB";
        public const string CHARGENAME = "Block Cleaning";
        public const ChargeGroup CHARGEGROUP = ChargeGroup.Tenants;
        public const ChargeType CHARGETYPE = ChargeType.Block;

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
        public static ChargeMaintenance ConstructChargeMaintenanceFromConstants()
        {
            var entity = new ChargeMaintenance
            {
                Id = ID,
                ChargesId = TARGETID,
                Reason = "Uplift",
                NewValue = new[]
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
                },
                ExistingValue = new[]
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
                },
                StartDate = new DateTime(2021, 7, 2),
                Status = ChargeMaintenanceStatus.Pending
            };
            return entity;
        }
        public static ChargesList ConstructChargeListFromConstants()
        {
            var entity = new ChargesList
            {
                Id = ID,
                ChargeCode = CHARGECODE,
                ChargeGroup = CHARGEGROUP,
                ChargeName = CHARGENAME,
                ChargeType = CHARGETYPE
            };
            return entity;
        }
        public static ChargeItem ConstructChargeItemFromConstants()
        {
            var entity = new ChargeItem
            {
                ChargeCode = CHARGECODE,
                IsChargeApplicable = true,
                ChargeName = CHARGENAME,
                PerPropertyCharge = AMOUNT
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
