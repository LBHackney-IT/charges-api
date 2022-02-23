using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways.Services;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.Helpers;
using ChargesApi.V1.UseCase.Interfaces;
using ExcelDataReader;
using Hackney.Shared.Asset.Domain;
using Microsoft.AspNetCore.Http;

namespace ChargesApi.V1.UseCase
{
    public class GeneratePropertyChargesFileUseCase : IGeneratePropertyChargesFileUseCase
    {
        private readonly IGetPropertyChargesUseCase _getPropertyChargesUseCase;
        private readonly IHousingSearchService _housingSearchService;
        private readonly IAwsS3FileService _awsS3FileService;

        public GeneratePropertyChargesFileUseCase(IGetPropertyChargesUseCase getPropertyChargesUseCase, IHousingSearchService housingSearchService, IAwsS3FileService awsS3FileService)
        {
            _getPropertyChargesUseCase = getPropertyChargesUseCase;
            _housingSearchService = housingSearchService;
            _awsS3FileService = awsS3FileService;
        }

        public async Task ExecuteAsync(PropertyChargesQueryParameters queryParameters)
        {
            var propertyCharges = await _getPropertyChargesUseCase.ExecuteAsync(queryParameters).ConfigureAwait(false);

            if (propertyCharges is null)
                throw new Exception("charges not found");

            // Retrieve assets list from housing search API
            var assetsList = await GetAssetsList(AssetType.Dwelling.ToString()).ConfigureAwait(false);
            var dwellingsListResult = assetsList.Item1;

            var bucketName = Environment.GetEnvironmentVariable("CHARGES_BUCKET_NAME");

            // Gets the file list
            var generatedFiles = await _awsS3FileService.GetProcessedFiles().ConfigureAwait(false);

            // gets the path of latest estimate file
            var filePath = generatedFiles.OrderByDescending(f => f.DateUploaded).Select(f => f.FileName).FirstOrDefault();
            var fileStream = await _awsS3FileService.GetFile(bucketName, filePath).ConfigureAwait(false);
            var fileResponse = ReadFile(fileStream);

            var builder = new StringBuilder();

            // Set header for csv file
            builder.AppendLine(CsvFileConstants.FileHeader);

            foreach (var propertyCharge in propertyCharges)
            {
                var assetId = dwellingsListResult.FirstOrDefault(x => x.Id == propertyCharge.Id)?.AssetId;

                var estimateActualCharge = fileResponse.FirstOrDefault(x =>
                    x.PropertyReferenceNumber == assetId);

                if (estimateActualCharge == null)
                    continue;

                // Update the charges value 
                var tenureData = UpdateEstimateActualCharge(estimateActualCharge, propertyCharge.DetailedCharges);

                builder.AppendLine(
                    $"{propertyCharge.Id}," +
                    $"{tenureData.PaymentReferenceNumber}," +
                    $"{tenureData.PropertyReferenceNumber}," +
                    $"{propertyCharge.ChargeGroup}," +
                    $"{propertyCharge.ChargeYear}," +
                    $"{propertyCharge.ChargeSubGroup}," +
                    $"{tenureData?.Name1}," +
                    $"{tenureData?.AddressLine1}," +
                    $"{tenureData?.AddressLine2}," +
                    $"{tenureData?.AddressLine3}," +
                    $"{tenureData?.AddressLine4}," +
                    $"{tenureData?.TotalCharge}," +
                    $"{tenureData?.BlockCCTVMaintenanceAndMonitoring}," +
                    $"{tenureData?.BlockCleaning}," +
                    $"{tenureData?.BlockElectricity}," +
                    $"{tenureData?.BlockRepairs}," +
                    $"{tenureData?.BuildingInsurancePremium}," +
                    $"{tenureData?.DoorEntry}," +
                    $"{tenureData?.CommunalTVAerialMaintenance}," +
                    $"{tenureData?.ConciergeService}," +
                    $"{tenureData?.EstateCCTVMaintenanceAndMonitoring}," +
                    $"{tenureData?.EstateCleaning}," +
                    $"{tenureData?.EstateElectricity}," +
                    $"{tenureData?.EstateRepairs}," +
                    $"{tenureData?.EstateRoadsFootpathsAndDrainage}," +
                    $"{tenureData?.GroundRent}," +
                    $"{tenureData?.GroundsMaintenance}," +
                    $"{tenureData?.HeatingOrHotWaterEnergy}," +
                    $"{tenureData?.HeatingOrHotWaterMaintenance}," +
                    $"{tenureData?.HeatingStandingCharge}," +
                    $"{tenureData?.LiftMaintenance}," +
                    $"{tenureData?.ManagementCharge}," +
                    $"{tenureData?.ReserveFund}");
            }

            var csvFileStream = new MemoryStream(Encoding.UTF8.GetBytes(builder.ToString()));

            var fileName = $"Charges{DateTime.Now:yyyyMMddHHmmssfff}.csv";
            var formFile = new FormFile(csvFileStream, 0, csvFileStream.Length, "chargesFile", fileName)
            {
                ContentType = "text/csv"
            };
            await _awsS3FileService.UploadPrintRoomFile(formFile, formFile.FileName).ConfigureAwait(false);
        }

        private async Task<(List<Asset>, long)> GetAssetsList(string assetType)
        {
            var pageNumber = Constants.Page;
            var pageSize = Constants.PageSize;

            var filteredList = new List<Asset>();

            var assetList = await _housingSearchService.GetAssets(assetType, pageSize, pageNumber, null)
                .ConfigureAwait(false);

            if (assetList is null) throw new Exception("Housing Search Service is not returning any asset list response");

            var totalPropertiesCount = assetList.Total;

            var lastHitId = assetList.LastHitId;

            filteredList.AddRange(assetList.Results.Assets);

            for (var i = 0; i < (assetList.Total / pageSize); i++)
            {
                pageNumber = i + 2;
                assetList = await _housingSearchService
                    .GetAssets(assetType, pageSize, pageNumber, lastHitId).ConfigureAwait(false);
                if (assetList is null) throw new Exception("Housing Search Service is not returning any asset list response");
                lastHitId = assetList.LastHitId;
                filteredList.AddRange(assetList.Results.Assets);
            }

            return (filteredList, totalPropertiesCount);
        }

        private static List<EstimateActualCharge> ReadFile(Stream fileStream)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var estimatesActual = new List<EstimateActualCharge>();
            // Read Excel
            using (var stream = new MemoryStream())
            {
                fileStream.CopyTo(stream);
                stream.Position = 1;

                // Excel Read Process
                using var reader = ExcelReaderFactory.CreateReader(stream);

                while (reader.Read())
                {
                    try
                    {
                        estimatesActual.Add(new EstimateActualCharge
                        {
                            PaymentReferenceNumber = reader.GetValue(0).ToString(),
                            PropertyReferenceNumber = reader.GetValue(1).ToString(),
                            TenureType = reader.GetValue(3).ToString(),
                            Name1 = reader.GetValue(8).ToString(),
                            AddressLine1 = reader.GetValue(9).ToString(),
                            AddressLine2 = reader.GetValue(10).ToString(),
                            AddressLine3 = reader.GetValue(11).ToString(),
                            AddressLine4 = reader.GetValue(12).ToString(),
                            TotalCharge = GetChargeAmount(reader.GetValue(18)),
                            BlockCCTVMaintenanceAndMonitoring = GetChargeAmount(reader.GetValue(19)),
                            BlockCleaning = GetChargeAmount(reader.GetValue(20)),
                            BlockElectricity = GetChargeAmount(reader.GetValue(21)),
                            BlockRepairs = GetChargeAmount(reader.GetValue(22)),
                            BuildingInsurancePremium = GetChargeAmount(reader.GetValue(23)),
                            DoorEntry = GetChargeAmount(reader.GetValue(24)),
                            CommunalTVAerialMaintenance = GetChargeAmount(reader.GetValue(25)),
                            ConciergeService = GetChargeAmount(reader.GetValue(26)),
                            EstateCCTVMaintenanceAndMonitoring = GetChargeAmount(reader.GetValue(27)),
                            EstateCleaning = GetChargeAmount(reader.GetValue(28)),
                            EstateElectricity = GetChargeAmount(reader.GetValue(29)),
                            EstateRepairs = GetChargeAmount(reader.GetValue(30)),
                            EstateRoadsFootpathsAndDrainage = GetChargeAmount(reader.GetValue(31)),
                            GroundRent = GetChargeAmount(reader.GetValue(32)),
                            GroundsMaintenance = GetChargeAmount(reader.GetValue(33)),
                            HeatingOrHotWaterEnergy = GetChargeAmount(reader.GetValue(34)),
                            HeatingOrHotWaterMaintenance = GetChargeAmount(reader.GetValue(35)),
                            HeatingStandingCharge = GetChargeAmount(reader.GetValue(36)),
                            LiftMaintenance = GetChargeAmount(reader.GetValue(37)),
                            ManagementCharge = GetChargeAmount(reader.GetValue(38)),
                            ReserveFund = GetChargeAmount(reader.GetValue(39))
                        });
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
            }

            return estimatesActual;
        }

        private static decimal GetChargeAmount(object excelColumnValue)
        {
            decimal result;
            if (excelColumnValue == null || excelColumnValue.ToString() == " ")
                result = 0;
            else
                result = Convert.ToDecimal(excelColumnValue);
            return result;
        }

        private static EstimateActualCharge UpdateEstimateActualCharge(EstimateActualCharge estimateActualCharge, IEnumerable<DetailedCharges> detailedCharges)
        {
            if (detailedCharges != null)
            {
                estimateActualCharge.BlockCCTVMaintenanceAndMonitoring = detailedCharges.FirstOrDefault(x => x.SubType == "Block CCTV Maintenance").Amount;
                estimateActualCharge.BlockCleaning = detailedCharges.FirstOrDefault(x => x.SubType == "Block Cleaning").Amount;
                estimateActualCharge.BlockElectricity = detailedCharges.FirstOrDefault(x => x.SubType == "Block Electricity").Amount;
                estimateActualCharge.BlockRepairs = detailedCharges.FirstOrDefault(x => x.SubType == "Block Repairs").Amount;
                estimateActualCharge.DoorEntry = detailedCharges.FirstOrDefault(x => x.SubType == "Communal Door Entry Maintenance").Amount;
                estimateActualCharge.CommunalTVAerialMaintenance = detailedCharges.FirstOrDefault(x => x.SubType == "Communal TV aerial Maintenance").Amount;
                estimateActualCharge.ConciergeService = detailedCharges.FirstOrDefault(x => x.SubType == "Concierge Service").Amount;
                estimateActualCharge.EstateCCTVMaintenanceAndMonitoring = detailedCharges.FirstOrDefault(x => x.SubType == "CCTV Maintenance").Amount;
                estimateActualCharge.EstateCleaning = detailedCharges.FirstOrDefault(x => x.SubType == "Estate Cleaning").Amount;
                estimateActualCharge.EstateElectricity = detailedCharges.FirstOrDefault(x => x.SubType == "Estate Electricity").Amount;
                estimateActualCharge.EstateRepairs = detailedCharges.FirstOrDefault(x => x.SubType == "Estate Repairs").Amount;
                estimateActualCharge.EstateRoadsFootpathsAndDrainage = detailedCharges.FirstOrDefault(x => x.SubType == "Roads, footpaths and drainage").Amount;
                estimateActualCharge.GroundsMaintenance = detailedCharges.FirstOrDefault(x => x.SubType == "Grounds Maintenance").Amount;
                estimateActualCharge.HeatingOrHotWaterEnergy = detailedCharges.FirstOrDefault(x => x.SubType == "Heating/Hotwater Energy").Amount;
                estimateActualCharge.HeatingOrHotWaterMaintenance = detailedCharges.FirstOrDefault(x => x.SubType == "Heating/Hotwater Maintenance").Amount;
                estimateActualCharge.HeatingStandingCharge = detailedCharges.FirstOrDefault(x => x.SubType == "Heating/Hotwater Standing Charge").Amount;
                estimateActualCharge.LiftMaintenance = detailedCharges.FirstOrDefault(x => x.SubType == "Lift Maintenance").Amount;
            }

            return estimateActualCharge;
        }
    }
}
