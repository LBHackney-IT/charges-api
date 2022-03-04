using Amazon.S3.Model;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.UseCase.Helpers;
using ChargesApi.V1.UseCase.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ChargesApi.V1.Helpers;

namespace ChargesApi.V1.UseCase
{
    public class EstimateActualUploadUseCase : IEstimateActualUploadUseCase
    {
        private readonly ILogger<EstimateActualUploadUseCase> _logger;
        private readonly IAwsS3FileService _s3FileService;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public EstimateActualUploadUseCase(
            ILogger<EstimateActualUploadUseCase> logger,
            IAwsS3FileService s3FileService,
            ISnsGateway snsGateway,
            ISnsFactory snsFactory)
        {
            _logger = logger;
            _s3FileService = s3FileService;
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task<bool> ExecuteAsync(IFormFile file, ChargeGroup chargeGroup, string token)
        {
            var excelData = new List<EstimateActualCharge>();
            var recordsCount = 0;
            var chargeYear = 0; ;
            var chargeSubGroup = string.Empty;

            // Validate Excel File by reading untill the end record
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var bytes = await file.GetBytes().ConfigureAwait(false);
            using (var stream = new MemoryStream(bytes))
            {
                _logger.LogDebug($"Starting reading the Excel File");

                stream.Position = 1;

                // Excel Read Process
                using var reader = ExcelReaderFactory.CreateReader(stream);

                while (reader.Read())
                {
                    if (reader.GetValue(1) != null)
                    {
                        if (recordsCount == 0)
                        {
                            chargeYear = Convert.ToInt16($"20{reader.GetValue(19).ToString().Substring(0, 2)}");
                            chargeSubGroup = reader.GetValue(19).ToString().Substring(0, 3).EndsWith("E")
                                               ? Constants.EstimateTypeFile
                                               : Constants.ActualTypeFile;
                            _logger.LogDebug($"Extracted the ChargeYear for Estimates Upload as {chargeYear}");
                        }
                        else
                        {
                            try
                            {
                                excelData.Add(new EstimateActualCharge
                                {
                                    PropertyReferenceNumber = reader.GetValue(1).ToString(),
                                    AssetAddress = reader.GetValue(2).ToString(),
                                    TenureType = reader.GetValue(3).ToString(),
                                    BlockId = reader.GetValue(4).ToString(),
                                    BlockAddress = reader.GetValue(5) != null ? reader.GetValue(5).ToString() : string.Empty,
                                    EstateId = reader.GetValue(6) != null ? reader.GetValue(6).ToString() : string.Empty,
                                    EstateAddress = reader.GetValue(7) != null ? reader.GetValue(7).ToString() : string.Empty,
                                    TotalCharge = FileReaderHelper.GetChargeAmount(reader.GetValue(18)),
                                    BlockCCTVMaintenanceAndMonitoring = FileReaderHelper.GetChargeAmount(reader.GetValue(19)),
                                    BlockCleaning = FileReaderHelper.GetChargeAmount(reader.GetValue(20)),
                                    BlockElectricity = FileReaderHelper.GetChargeAmount(reader.GetValue(21)),
                                    BlockRepairs = FileReaderHelper.GetChargeAmount(reader.GetValue(22)),
                                    BuildingInsurancePremium = FileReaderHelper.GetChargeAmount(reader.GetValue(23)),
                                    DoorEntry = FileReaderHelper.GetChargeAmount(reader.GetValue(24)),
                                    CommunalTVAerialMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(25)),
                                    ConciergeService = FileReaderHelper.GetChargeAmount(reader.GetValue(26)),
                                    EstateCCTVMaintenanceAndMonitoring = FileReaderHelper.GetChargeAmount(reader.GetValue(27)),
                                    EstateCleaning = FileReaderHelper.GetChargeAmount(reader.GetValue(28)),
                                    EstateElectricity = FileReaderHelper.GetChargeAmount(reader.GetValue(29)),
                                    EstateRepairs = FileReaderHelper.GetChargeAmount(reader.GetValue(30)),
                                    EstateRoadsFootpathsAndDrainage = FileReaderHelper.GetChargeAmount(reader.GetValue(31)),
                                    GroundRent = FileReaderHelper.GetChargeAmount(reader.GetValue(32)),
                                    GroundsMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(33)),
                                    HeatingOrHotWaterEnergy = FileReaderHelper.GetChargeAmount(reader.GetValue(34)),
                                    HeatingOrHotWaterMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(35)),
                                    HeatingStandingCharge = FileReaderHelper.GetChargeAmount(reader.GetValue(36)),
                                    LiftMaintenance = FileReaderHelper.GetChargeAmount(reader.GetValue(37)),
                                    ManagementCharge = FileReaderHelper.GetChargeAmount(reader.GetValue(38)),
                                    ReserveFund = FileReaderHelper.GetChargeAmount(reader.GetValue(39))
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
                }
                _logger.LogDebug($"Reading Estimates Excel Sheet successful with total record count : {recordsCount - 1}");
            }

            // Upload file to S3 and raise Event FileUplaodedEvent
            var fileName = string.Concat(
                Path.GetFileNameWithoutExtension(file.FileName),
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(file.FileName)
            );
            var fileTags = new List<Tag>()
            {
                new Tag {Key = "year", Value = $"{chargeYear}"},
                new Tag {Key = "valuesType", Value = $"{chargeSubGroup}"}
            };
            var uploadResponse = await _s3FileService.UploadFile(file, fileName, fileTags).ConfigureAwait(false);
            var snsMessage = _snsFactory.CreateFileUploadMessage(uploadResponse);
            await _snsGateway.Publish(snsMessage).ConfigureAwait(false);

            _logger.LogDebug($"File successfully pushed to S3 bucket");

            // All Steps Evaluation Result
            if (chargeYear > 0 && !string.IsNullOrEmpty(chargeSubGroup) && excelData.Count == recordsCount - 1)
                return true;
            else
                return false;
        }
    }
}
