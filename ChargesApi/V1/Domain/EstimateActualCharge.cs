using System;

namespace ChargesApi.V1.Domain
{
    public class EstimateActualCharge
    {
        public string PaymentReferenceNumber { get; set; }
        public string PropertyReferenceNumber { get; set; }
        public Guid AssetId { get; set; }
        public string AssetAddress { get; set; }
        public string BlockId { get; set; }
        public string BlockAddress { get; set; }
        public string EstateId { get; set; }
        public string EstateAddress { get; set; }
        public string TenureType { get; set; }
        public decimal TotalCharge { get; set; }
        public decimal BlockCCTVMaintenanceAndMonitoring { get; set; }
        public decimal BlockCleaning { get; set; }
        public decimal BlockElectricity { get; set; }
        public decimal BlockRepairs { get; set; }
        public decimal BuildingInsurancePremium { get; set; }
        public decimal DoorEntry { get; set; }
        public decimal CommunalTVAerialMaintenance { get; set; }
        public decimal ConciergeService { get; set; }
        public decimal EstateCCTVMaintenanceAndMonitoring { get; set; }
        public decimal EstateCleaning { get; set; }
        public decimal EstateElectricity { get; set; }
        public decimal EstateRepairs { get; set; }
        public decimal EstateRoadsFootpathsAndDrainage { get; set; }
        public decimal GroundRent { get; set; }
        public decimal GroundsMaintenance { get; set; }
        public decimal HeatingOrHotWaterEnergy { get; set; }
        public decimal HeatingOrHotWaterMaintenance { get; set; }
        public decimal HeatingStandingCharge { get; set; }
        public decimal LiftMaintenance { get; set; }
        public decimal ManagementCharge { get; set; }
        public decimal ReserveFund { get; set; }
        public string Name1 { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PropertyAddress { get; set; }
    }
}
