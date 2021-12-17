using ChargesApi.V1.Domain;
using System.Threading.Tasks;

namespace ChargesApi.V1.Gateways
{
    public interface ISnsGateway
    {
        Task Publish(ChargesSns chargesSns);

    }
}
