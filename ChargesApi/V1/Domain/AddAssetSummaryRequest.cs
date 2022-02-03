using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Domain
{
    public class AddAssetSummaryRequest
    {
        public Guid TargetId { get; set; }
        
        public TargetType TargetType { get; set; }
       
        public string AssetName { get; set; }
      
        public decimal TotalServiceCharges { get; set; }

        public DateTime SubmitDate { get; set; }

        public short SumamryYear { get; set; }

        public int TotalLeaseholders { get; set; }

        public int TotalFreeholders { get; set; }

        public int TotalDwellings { get; set; }
    }
}
