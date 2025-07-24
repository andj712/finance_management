namespace finance_management.DTOs.ImportTransaction
{
    public class TransactionCsvDto
    {
        //ovo su sve vrednosti direktno iz csv fajla
        public string Id { get; set; } = string.Empty;
        public string? BeneficiaryName { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? Mcc { get; set; }
        public string Kind { get; set; } = string.Empty;
    }
}
