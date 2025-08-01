using System.Text.Json.Serialization;

namespace finance_management.DTOs.GetTransactions
{
    public class SingleCategorySplit
    {
        [JsonPropertyName("catcode")]
        public string CatCode { get; set; } = string.Empty;

        public double Amount { get; set; }
    }
}
