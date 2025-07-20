namespace finance_management.DTOs
{
    public class TransactionCsvDto
    {
        public string Id { get; set; }
        public string BeneficiaryName { get; set; }
        public string Date { get; set; }
        public string Direction { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string Mcc { get; set; }
        public string Kind { get; set; }
    }
}
