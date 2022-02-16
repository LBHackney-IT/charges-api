using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Domain
{
    public class AssetListResponse
    {
        public Results Results { get; set; }
        public long Total { get; set; }
        public string LastHitId { get; set; }
    }
}
