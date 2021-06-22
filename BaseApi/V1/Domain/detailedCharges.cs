using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeApi.V1.Domain
{
    public class DetailedCharges
    {
        private decimal _amount;

        public string Type { get; set; }
        public string SubType { get; set; }
        public string Frequency { get; set; }
        public decimal Amount {
            get {
                return _amount;
            }
            set {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Amount","Please enter the valid value");
                }
                _amount = value;
            }
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
