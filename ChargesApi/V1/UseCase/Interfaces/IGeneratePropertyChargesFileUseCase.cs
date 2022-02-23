using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChargesApi.V1.Boundary.Request;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.UseCase.Interfaces
{
    public interface IGeneratePropertyChargesFileUseCase
    {
        Task ExecuteAsync(PropertyChargesQueryParameters queryParameters);
    }
}
