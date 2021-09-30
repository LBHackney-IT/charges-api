using ChargesApi.V1.Domain;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.UseCase.Interfaces;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase
{
    public class AddActualsUseCase : IAddActualsUseCase
    {
        private readonly IActualsApiGateway _actualsApiGateway;

        public AddActualsUseCase(IActualsApiGateway actualsApiGateway)
        {
            _actualsApiGateway = actualsApiGateway;
        }

        public async Task<int> AddActuals(IFormFile file)
        {
            List<Actual> actuals = new List<Actual>();
            int processingCount = 0;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 1;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read()) 
                    {
                        if (processingCount != 0)
                        {
                            actuals.Add(new Actual
                            {
                                Name = reader.GetValue(0).ToString(),
                                Prn = reader.GetValue(1).ToString(),
                                BlockName = reader.GetValue(2).ToString(),
                                EstateName = reader.GetValue(3).ToString(),
                                MonthlyAmount = Convert.ToDecimal(reader.GetValue(4)),
                                YearlyAmount = Convert.ToDecimal(reader.GetValue(5)),
                                ActualYear = Convert.ToInt16(reader.GetValue(6))
                            });
                        }
                        processingCount++;
                    }
                }
            }

            var result = await _actualsApiGateway.SaveActualsBatch(actuals).ConfigureAwait(false);
            if (result)
                return processingCount - 1;
            else
                return 0;
        }
    }
}
