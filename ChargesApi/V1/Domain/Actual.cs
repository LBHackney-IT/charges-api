namespace ChargesApi.V1.Domain
{
    public class Actual
    {
        public string Name { get; set; }

        public string Prn { get; set; }

        public string BlockName { get; set; }

        public string EstateName { get; set; }

        public decimal MonthlyAmount { get; set; }

        public decimal YearlyAmount { get; set; }

        public short ActualYear { get; set; }
    }
}
