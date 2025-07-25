using System.Text.Json.Serialization;

namespace finance_management.DTOs
{
    public class SplitDto
    {
        [JsonPropertyName("catcode")]
        public string CatCode { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}
