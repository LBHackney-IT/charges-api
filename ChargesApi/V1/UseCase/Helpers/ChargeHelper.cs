using ChargesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Helpers
{
    public static class ChargeHelper
    {
        public static DetailedCharges GetChargeDetailModel(decimal chargeAmount,
          string chargeName, string chargeCode, ChargeGroup chargeGroup, ChargeType chargeType)
        {
            return new DetailedCharges
            {
                Type = Constants.ServiceChargeType,
                SubType = chargeName,
                ChargeCode = chargeCode,
                Amount = chargeAmount,
                ChargeType = chargeType,
                Frequency = chargeGroup == ChargeGroup.Tenants
                            ? ChargeFrequency.Weekly.ToString()
                            : ChargeFrequency.Monthly.ToString(),
                StartDate = Helper.GetFirstMondayForApril(DateTime.UtcNow.Year),
                EndDate = Helper.GetFirstMondayForApril(DateTime.UtcNow.AddYears(1).Year).AddDays(-1)
            };
        }

        public static ChargeType GetChargeType(string chargeName)
        {
            switch (chargeName)
            {
                case "Estate Cleaning":
                case "Estate Repairs":
                case "Estate Electricity":
                case "Estate Roads Footpaths and Drainage":
                case "Estate CCTV Maintenance and Monitoring":
                case "Grounds Maintenance":
                    return ChargeType.Estate;
                case "Block Cleaning":
                case "Block Repairs":
                case "Block Electricity":
                case "Door Entry":
                case "Lift Maintenance":
                case "Communal TV Aerial Maintenance":
                case "Heating/Hotwater Maintenance":
                case "Heating/Hotwater Standing Charge":
                case "Heating/Hotwater Energy":
                case "Block CCTV Maintenance and Monitoring":
                case "Concierge Service":
                    return ChargeType.Block;
                default:
                    return ChargeType.Property;
            }
        }
        public static Charge GetChargeModel(string assetType, ChargeGroup chargeGroup, string createdBy,
           short chargeYear, EstimateCharge estimateCharge)
        {
            var detailedChargesList = new List<DetailedCharges>();
            if (estimateCharge.BlockCCTVMaintenanceAndMonitoring >= 0)
            {
                var chargeType = GetChargeType(Constants.BlockCCTVMaintenanceAndMonitoring);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.BlockCCTVMaintenanceAndMonitoring,
                    Constants.BlockCCTVMaintenanceAndMonitoring, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.BlockCleaning >= 0)
            {
                var chargeType = GetChargeType(Constants.BlockCleaning);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.BlockCleaning,
                    Constants.BlockCleaning, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.BlockElectricity >= 0)
            {
                var chargeType = GetChargeType(Constants.BlockElectricity);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.BlockElectricity,
                    Constants.BlockElectricity, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.BlockRepairs >= 0)
            {
                var chargeType = GetChargeType(Constants.BlockRepairs);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.BlockRepairs,
                    Constants.BlockRepairs, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.BuildingInsurancePremium >= 0)
            {
                var chargeType = GetChargeType(Constants.BuildingInsurancePremium);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.BuildingInsurancePremium,
                    Constants.BuildingInsurancePremium, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.CommunalTVAerialMaintenance >= 0)
            {
                var chargeType = GetChargeType(Constants.CommunalTVAerialMaintenance);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.CommunalTVAerialMaintenance,
                    Constants.CommunalTVAerialMaintenance, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.ConciergeService >= 0)
            {
                var chargeType = GetChargeType(Constants.ConciergeService);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.ConciergeService,
                    Constants.ConciergeService, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.DoorEntry >= 0)
            {
                var chargeType = GetChargeType(Constants.DoorEntry);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.DoorEntry,
                    Constants.DoorEntry, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.EstateCCTVMaintenanceAndMonitoring >= 0)
            {
                var chargeType = GetChargeType(Constants.EstateCCTVMaintenanceAndMonitoring);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.EstateCCTVMaintenanceAndMonitoring,
                    Constants.EstateCCTVMaintenanceAndMonitoring, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.EstateCleaning >= 0)
            {
                var chargeType = GetChargeType(Constants.EstateCleaning);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.EstateCleaning,
                    Constants.EstateCleaning, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.EstateElectricity >= 0)
            {
                var chargeType = GetChargeType(Constants.EstateElectricity);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.EstateElectricity,
                    Constants.EstateElectricity, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.EstateRepairs >= 0)
            {
                var chargeType = GetChargeType(Constants.EstateRepairs);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.EstateRepairs,
                    Constants.EstateRepairs, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.EstateRoadsFootpathsAndDrainage >= 0)
            {
                var chargeType = GetChargeType(Constants.EstateRoadsFootpathsAndDrainage);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.EstateRoadsFootpathsAndDrainage,
                    Constants.EstateRoadsFootpathsAndDrainage, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.GroundRent >= 0)
            {
                var chargeType = GetChargeType(Constants.GroundRent);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.GroundRent,
                    Constants.GroundRent, Constants.GroundRentChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.GroundsMaintenance >= 0)
            {
                var chargeType = GetChargeType(Constants.GroundsMaintenance);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.GroundsMaintenance,
                    Constants.GroundsMaintenance, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.HeatingOrHotWaterEnergy >= 0)
            {
                var chargeType = GetChargeType(Constants.HeatingOrHotWaterEnergy);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.HeatingOrHotWaterEnergy,
                    Constants.HeatingOrHotWaterEnergy, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.HeatingOrHotWaterMaintenance >= 0)
            {
                var chargeType = GetChargeType(Constants.HeatingOrHotWaterMaintenance);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.HeatingOrHotWaterMaintenance,
                    Constants.HeatingOrHotWaterMaintenance, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.HeatingStandingCharge >= 0)
            {
                var chargeType = GetChargeType(Constants.HeatingStandingCharge);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.HeatingStandingCharge,
                    Constants.HeatingStandingCharge, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.LiftMaintenance >= 0)
            {
                var chargeType = GetChargeType(Constants.LiftMaintenance);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.LiftMaintenance,
                    Constants.LiftMaintenance, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.ManagementCharge >= 0)
            {
                var chargeType = GetChargeType(Constants.ManagementCharge);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.ManagementCharge,
                    Constants.ManagementCharge, Constants.EstimateChargeCode, chargeGroup, chargeType));
            }
            if (estimateCharge.ReserveFund >= 0)
            {
                var chargeType = GetChargeType(Constants.ReserveFund);
                detailedChargesList.Add(GetChargeDetailModel(estimateCharge.ReserveFund,
                    Constants.ReserveFund, Constants.ReserveFundChargeCode, chargeGroup, chargeType));
            }

            var newCharge = new Charge
            {
                Id = Guid.NewGuid(),
                TargetId = estimateCharge.AssetId,
                ChargeGroup = chargeGroup,
                ChargeYear = chargeYear,
                TargetType = (TargetType) Enum.Parse(typeof(TargetType), assetType),
                DetailedCharges = detailedChargesList.AsEnumerable(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
            return newCharge;

        }
    }
}
