using System.Text.Json.Serialization;

namespace finance_management.DTOs
{
    public class TransactionWithSplitsDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("beneficiary-name")]
        public string? BeneficiaryName { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("direction")]
        public string Direction { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("mcc")]
        public int? MccCode { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; } = string.Empty;

        [JsonPropertyName("catcode")]
        public string? CatCode { get; set; }

        [JsonPropertyName("splits")]
        public List<SplitDto> Splits { get; set; } = new();
    }
}
