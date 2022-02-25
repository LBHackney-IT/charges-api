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

            // Gets all the file list
            var generatedFiles = await _awsS3FileService.GetProcessedFiles().ConfigureAwait(false);
            var orderedFiles = generatedFiles.OrderByDescending(f => f.DateUploaded);
            var filePath = string.Empty;

            // Find the filtered estimated file by charge year and charge sub group
            foreach (var file in orderedFiles)
            {
                // Check the file tag
                if (file.Year == queryParameters.ChargeYear.ToString() && file.ValuesType == queryParameters.ChargeSubGroup.ToString())
                {
                    var stream = await _awsS3FileService.GetFile(file.FileName).ConfigureAwait(false);

                    if (ReadHeader(stream, queryParameters.ChargeYear, queryParameters.ChargeSubGroup))
                    {
                        filePath = file.FileName;
                    }
                }
            }

            var fileStream = await _awsS3FileService.GetFile(filePath).ConfigureAwait(false);

            if (fileStream == null)
                throw new Exception("Cannot locate Estimate File");

            var fileResponse = ReadFile(fileStream);

            var builder = new StringBuilder();

            // Set header for csv file
            var fileHeader = Environment.GetEnvironmentVariable("PRINT_RENT_STATEMENTS_HEADER");
            builder.AppendLine(fileHeader);

            foreach (var propertyCharge in propertyCharges)
            {
                var assetId = dwellingsListResult.FirstOrDefault(x => x.Id == propertyCharge.Id)?.AssetId;

                var estimateActualCharge = fileResponse.FirstOrDefault(x =>
                    x.PropertyReferenceNumber == assetId);

                if (estimateActualCharge == null)
                    throw new Exception("Cannot locate the asset on estimate file");

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
                    $"{tenureData?.PropertyAddress}," +
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
            var formFile = new FormFile(csvFileStream, 0, csvFileStream.Length, "chargesFile", fileName);

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
            var recordsCount = 0;
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
                    if (recordsCount > 0)
                    {
                        try
                        {
                            estimatesActual.Add(new EstimateActualCharge
                            {
                                PaymentReferenceNumber = reader.GetValue(0).ToString(),
                                PropertyReferenceNumber = reader.GetValue(1).ToString(),
                                PropertyAddress = reader.GetValue(2).ToString(),
                                TenureType = reader.GetValue(3).ToString(),
                                Name1 = reader.GetValue(8).ToString(),
                                AddressLine1 = reader.GetValue(9).ToString(),
                                AddressLine2 = reader.GetValue(10).ToString(),
                                AddressLine3 = reader.GetValue(11).ToString(),
                                AddressLine4 = reader.GetValue(12).ToString(),
                                TotalCharge = FileReaderHelper.GetChargeAmount(reader.GetValue(18)),
                                BlockCCTVMaintenanceAndMonitoring =
                                    FileReaderHelper.GetChargeAmount(reader.GetValue(19)),
                                BlockCleaning = FileReaderHelper.GetChargeAmount(reader.GetValue(20)),
                                BlockElectricity = FileReaderHelper.GetChargeAmount(reader.GetValue(21)),
                                BlockRepairs = FileReaderHelper.GetChargeAmount(reader.GetValue(22)),
                                BuildingInsurancePremium = FileReaderHelper.GetChargeAmount(reader.GetValue(23)),
                                DoorEntry = FileReaderHelper.GetChargeAmount(reader.GetValue(24)),
                                CommunalTVAerialMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(25)),
                                ConciergeService = FileReaderHelper.GetChargeAmount(reader.GetValue(26)),
                                EstateCCTVMaintenanceAndMonitoring =
                                    FileReaderHelper.GetChargeAmount(reader.GetValue(27)),
                                EstateCleaning = FileReaderHelper.GetChargeAmount(reader.GetValue(28)),
                                EstateElectricity = FileReaderHelper.GetChargeAmount(reader.GetValue(29)),
                                EstateRepairs = FileReaderHelper.GetChargeAmount(reader.GetValue(30)),
                                EstateRoadsFootpathsAndDrainage =
                                    FileReaderHelper.GetChargeAmount(reader.GetValue(31)),
                                GroundRent = FileReaderHelper.GetChargeAmount(reader.GetValue(32)),
                                GroundsMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(33)),
                                HeatingOrHotWaterEnergy = FileReaderHelper.GetChargeAmount(reader.GetValue(34)),
                                HeatingOrHotWaterMaintenance =
                                    FileReaderHelper.GetChargeAmount(reader.GetValue(35)),
                                HeatingStandingCharge = FileReaderHelper.GetChargeAmount(reader.GetValue(36)),
                                LiftMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(37)),
                                ManagementCharge = FileReaderHelper.GetChargeAmount(reader.GetValue(38)),
                                ReserveFund = FileReaderHelper.GetChargeAmount(reader.GetValue(39))
                            });
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }

                    recordsCount++;
                }
            }

            return estimatesActual;
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

        private static bool ReadHeader(Stream fileStream, short chargeYear, ChargeSubGroup chargeSubGroup)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            short fileHeaderChargeYear = 0;
            var fileHeaderChargeSubGroup = string.Empty;
            var recordsCount = 0;

            if (fileStream == null)
                return false;

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
                        if (recordsCount == 0)
                        {
                            fileHeaderChargeYear =
                                Convert.ToInt16($"20{reader.GetValue(19).ToString()?.Substring(0, 2)}");
                            fileHeaderChargeSubGroup = reader.GetValue(19).ToString().Substring(0, 3).EndsWith("E")
                                ? Constants.EstimateTypeFile
                                : Constants.ActualTypeFile;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                    recordsCount++;
                }
            }

            if (fileHeaderChargeYear == chargeYear && fileHeaderChargeSubGroup == chargeSubGroup.ToString())
            {
                return true;
            }

            return false;
        }
    }
}
