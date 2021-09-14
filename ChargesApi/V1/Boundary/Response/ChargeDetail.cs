namespace ChargesApi.V1.Boundary.Response
{
    public class ChargeDetail
    {
        public string ChargeName { get; set; }
        public string ChargeCode { get; set; }
        public decimal ChargeAmount { get; set; }
        public int ChargeYear { get; set; }
    }
}
