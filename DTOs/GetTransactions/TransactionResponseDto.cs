using finance_management.Models;
using finance_management.Models.Enums;
using Newtonsoft.Json;

namespace finance_management.DTOs.GetTransactions
{
    public class TransactionResponseDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("beneficiary-name")]
        public string BeneficiaryName { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("direction")]
        public DirectionEnum Direction { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("mcc-code")]
        public MccCodeEnum? MccCode { get; set; }

        [JsonProperty("kind")]
        public TransactionKindEnum Kind { get; set; }

        [JsonProperty("cat-code")]
        public string? CatCode { get; set; }

        [JsonProperty("category")]
        public Category? Category { get; set; }

        [JsonProperty("splits")]
        public List<SingleCategorySplit> Splits { get; set; }
    }
}
