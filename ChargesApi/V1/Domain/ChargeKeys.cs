using System;

namespace ChargesApi.V1.Domain
{
    public class ChargeKeys
    {
        public Guid Id { get; }
        public Guid TargetId { get; }

        public ChargeKeys(Guid id, Guid targetId)
        {
            Id = id;
            TargetId = targetId;
        }
    }
}
