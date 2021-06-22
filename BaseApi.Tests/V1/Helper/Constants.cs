using ChargeApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var entity = new Charge();
            entity.Id = Constants.ID;
            entity.TargetId = Constants.TARGETID;
            entity.TargetType = Constants.TARGETTYPE;
            entity.DetailedCharges = new[]
            {
                new DetailedCharges
                {
                    Amount = Constants.AMOUNT,
                    StartDate = DateTime.Parse(Constants.STARTDATE),
                    EndDate = DateTime.Parse(Constants.ENDDATE),
                    Frequency = Constants.FREQUENCY,
                    SubType = Constants.SUBTYPE,
                    Type = Constants.TYPE
                }
            };
            return entity;
        }

        public static DetailedCharges ConstructDetailedChargesFromConstants()
        {
            var entity = new DetailedCharges();
            entity.Amount = Constants.AMOUNT;
            entity.EndDate = DateTime.Parse(Constants.ENDDATE);
            entity.Frequency = Constants.FREQUENCY;
            entity.StartDate = DateTime.Parse(Constants.STARTDATE);
            entity.SubType = Constants.SUBTYPE;
            entity.Type = Constants.TYPE;
            return entity;
        }
    }
}
