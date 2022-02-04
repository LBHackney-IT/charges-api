using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.Infrastructure.JWT;
using ChargesApi.V1.UseCase.Helpers;
using ChargesApi.V1.UseCase.Interfaces;
using ExcelDataReader;
using Hackney.Shared.Asset.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class AddEstimateChargesUseCase : IAddEstimateChargesUseCase
    {
        private readonly IChargesApiGateway _chargesApiGateway;
        private readonly IHousingSearchService _housingSearchService;
        private readonly IFinancialSummaryService _financialSummaryService;
        private readonly ILogger<AddEstimateChargesUseCase> _logger;
        private static List<Asset> _assetData;

        public AddEstimateChargesUseCase(IChargesApiGateway chargesApiGateway,
            IHousingSearchService housingSearchService,
            IFinancialSummaryService financialSummaryService,
            ILogger<AddEstimateChargesUseCase> logger)
        {
            _chargesApiGateway = chargesApiGateway;
            _housingSearchService = housingSearchService;
            _financialSummaryService = financialSummaryService;
            _logger = logger;
        }
        public async Task<int> AddEstimates(IFormFile file, ChargeGroup chargeGroup, string token)
        {
            // Get Full Assets List

            if (_assetData == null)
            {
                _logger.LogDebug($"Starting fetching assets list from Housing Search API Asset Search Endpoint");
                var assetsList = await GetAssetsList(AssetType.Dwelling.ToString()).ConfigureAwait(false);

                _assetData = assetsList.Item1;
                _logger.LogDebug($"Assets List fetching completed and total assets fetched : {assetsList.Item1.Count}");
            }


            List<EstimateCharge> estimates = new List<EstimateCharge>();
            var recordsCount = 0;
            short chargeYear = 0; ;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var bytes = await file.GetBytes().ConfigureAwait(false);
            using (var stream = new MemoryStream(bytes))
            {
                _logger.LogDebug($"Starting reading the Excel File");

                //var bytes = await file.GetBytes().ConfigureAwait(false);
                //await bytes.CopyToAsync(stream).ConfigureAwait(false);

                //using (var data = new MemoryStream(fileBytes.ToArray()))

                stream.Position = 1;

                // Excel Read Process
                using var reader = ExcelReaderFactory.CreateReader(stream);

                while (reader.Read())
                {
                    if (recordsCount == 0)
                    {
                        chargeYear = Convert.ToInt16($"20{reader.GetValue(19).ToString().Substring(0, 2)}");
                        _logger.LogDebug($"Extracted the ChargeYear for Estimates Upload as {chargeYear}");
                    }
                    else
                    {
                        try
                        {
                            estimates.Add(new EstimateCharge
                            {
                                PropertyReferenceNumber = reader.GetValue(1).ToString(),
                                TenureType = reader.GetValue(3).ToString(),
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
                            _logger.LogDebug($"Exception occurred while reading the Estimates Excel Sheet: {e.Message}");
                            throw new Exception(e.Message);
                        }

                    }
                    recordsCount++;
                }
                _logger.LogDebug($"Reading Estimates Excel Sheet successfull with total record count : {recordsCount - 1}");
            }



            _logger.LogDebug($"Starting UH numerical Asset Id transformation with Guid Asset Id");

            // Charges Transformation
            estimates.ForEach(item =>
            {
                var data = _assetData.FirstOrDefault(x => x.AssetId == item.PropertyReferenceNumber);
                // TBC
                if (data == null)
                {
                    _logger.LogDebug($"Could not find associated Guid Asset Id for UH Asset Id : {item.PropertyReferenceNumber}");
                    item.AssetId = Guid.NewGuid();
                    _logger.LogDebug($"Created Asset Guid for UH Asset Id {item.PropertyReferenceNumber}: {item.AssetId}");
                }
                else
                    item.AssetId = data.Id;
            });

            var charges = new List<Charge>();
            var createdBy = Infrastructure.JWT.Helper.GetUserName(token);

            _logger.LogDebug($"Starting Charges formation Process");
            foreach (var item in estimates)
            {
                charges.Add(Helpers.ChargeHelper.GetChargeModel(AssetType.Dwelling.ToString(), chargeGroup, createdBy, chargeYear, item));
            }
            _logger.LogDebug($"Charges formation Process completed with total record count as : {charges.Count}");

            // Charges Load
            var maxBatchCount = Constants.PerBatchProcessingCount;
            bool loadResult = false;
            var loopCount = (charges.Count / maxBatchCount) + 1;
            var processedCount = 0;
            for (var start = 0; start < loopCount; start++)
            {
                var itemsToWrite = charges.Skip(start * maxBatchCount).Take(maxBatchCount);
                processedCount += itemsToWrite.Count();
                loadResult = await _chargesApiGateway.AddTransactionBatchAsync(itemsToWrite.ToList()).ConfigureAwait(false);
            }

            // Financial Summary Load
            var freeholdersCount = Helpers.Helper.GetFreeholdersCount(estimates);
            var leaseholdersCount = Helpers.Helper.GetLeaseholdersCount(estimates);
            var totalEstimateCharge = Convert.ToInt32(estimates.Sum(x => x.TotalCharge));

            var addSummaryRequest = new AddAssetSummaryRequest
            {
                TargetId = Guid.Parse(Constants.HackneyRootAssetId),
                SubmitDate = DateTime.UtcNow,
                AssetName = Constants.RootAsset,
                SumamryYear = chargeYear,
                TargetType = TargetType.NA,
                TotalServiceCharges = totalEstimateCharge,
                TotalDwellings = estimates.Count,
                TotalFreeholders = freeholdersCount,
                TotalLeaseholders = leaseholdersCount
            };
            var loadSummaryResult = await _financialSummaryService.AddEstimateSummary(addSummaryRequest).ConfigureAwait(false);

            _logger.LogDebug($"Charges loading Process completed with total record count loaded : {charges.Count}");

            // All Steps Evaluation
            if (loadResult && loadSummaryResult && processedCount == recordsCount - 1)
                return processedCount;
            else
                return 0;
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
    }
}
