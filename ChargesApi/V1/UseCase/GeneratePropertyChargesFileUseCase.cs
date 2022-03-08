using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;
using ChargesApi.V1.Factories;
using ChargesApi.V1.Gateways;
using ChargesApi.V1.Gateways.Services;
using ChargesApi.V1.Gateways.Services.Interfaces;
using ChargesApi.V1.Helpers;
using ChargesApi.V1.UseCase.Interfaces;
using ExcelDataReader;
using Hackney.Shared.Asset.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ChargesApi.V1.UseCase
{
    public class GeneratePropertyChargesFileUseCase : IGeneratePropertyChargesFileUseCase
    {
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;

        public GeneratePropertyChargesFileUseCase(ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _snsGateway = snsGateway;
            _snsFactory = snsFactory;
        }

        public async Task ExecuteAsync(PropertyChargesQueryParameters queryParameters)
        {
            var snsMessage = _snsFactory.UploadPrintRentRoomMessage(queryParameters);
            await _snsGateway.Publish(snsMessage).ConfigureAwait(false);
        }
    }
}
