using System.Text.Json.Serialization;

namespace finance_management.DTOs.GetTransactions
{
    public class SingleCategorySplit
    {
        [JsonPropertyName("cat-code")]
        public string CatCode { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public double Amount { get; set; }
    }
}
