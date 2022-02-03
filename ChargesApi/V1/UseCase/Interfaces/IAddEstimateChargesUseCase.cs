using ChargesApi.V1.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IAddEstimateChargesUseCase
    {
        Task<int> AddEstimates(IFormFile file, ChargeGroup chargeGroup, string token);
    }
}