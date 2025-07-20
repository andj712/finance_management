namespace finance_management.DTOs
{
    public class SplitTransactionRequest
    {
        public decimal SplitAmount { get; set; }
        public string? NewDescription { get; set; }
    }
}
