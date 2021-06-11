using ChargeApi.V1.Boundary.Response;
using ChargeApi.V1.Domain;
using System;
using System.Threading.Tasks;

namespace ChargeApi.V1.UseCase.Interfaces
{
    public interface ICalculateChargesUseCase
    {
        public void Execute(Guid targetid,string targettype);
        public Task ExecuteAsync(Guid targetid, string targettype);
    }
}
