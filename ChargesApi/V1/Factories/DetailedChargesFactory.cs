using Amazon.DynamoDBv2.Model;
using ChargesApi.V1.Domain;

namespace ChargesApi.V1.Factories
{
    public static class DetailedChargesFactory
    {
        public static DetailedCharges ToDomain(this AttributeValue scanResponseItem) => new DetailedCharges
        {

        };
    }
}
