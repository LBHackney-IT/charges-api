using System;

namespace ChargesApi.V1
{
    public static class Constants
    {
        public const string CorrelationId = "x-correlation-id";
        public const string ServiceChargeType = "Service";
        public const string ChargeTableName = "Charges";
        public const string AttributeNotExistId = "attribute_not_exists(id)";
        public const string HackneyRootAssetId = "656feda1-896f-b136-da84-163ee4f1be6c";
        public const string RootAsset = "RootAsset";
        public const int PageSize = 8000;
        public const int Page = 1;
        public const int PerBatchProcessingCount = 25;
        public const string UtcDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

        public const string EstimateChargeCode = "DSC";
        public const string ReserveFundChargeCode = "DSR";
        public const string GroundRentChargeCode = "DGR";

        public const string BlockCCTVMaintenanceAndMonitoring = "Block CCTV Maintenance and Monitoring";
        public const string BlockCleaning = "Block Cleaning";
        public const string BlockElectricity = "Block Electricity";
        public const string BlockRepairs = "Block Repairs";
        public const string BuildingInsurancePremium = "Building Insurance Premium";
        public const string DoorEntry = "Door Entry";
        public const string CommunalTVAerialMaintenance = "Communal TV Aerial Maintenance";
        public const string ConciergeService = "Concierge Service";
        public const string EstateCCTVMaintenanceAndMonitoring = "Estate CCTV Maintenance and Monitoring";
        public const string EstateCleaning = "Estate Cleaning";
        public const string EstateElectricity = "Estate Electricity";
        public const string EstateRepairs = "Estate Repairs";
        public const string EstateRoadsFootpathsAndDrainage = "Estate Roads Footpaths and Drainage";
        public const string GroundRent = "Ground Rent";
        public const string GroundsMaintenance = "Grounds Maintenance";
        public const string HeatingOrHotWaterEnergy = "Heating/Hotwater Energy";
        public const string HeatingOrHotWaterMaintenance = "Heating/Hotwater Maintenance";
        public const string HeatingStandingCharge = "Heating/Hotwater Standing Charge";
        public const string LiftMaintenance = "Lift Maintenance";
        public const string ManagementCharge = "Management Charge";
        public const string ReserveFund = "Reserve Fund";
    }
}
