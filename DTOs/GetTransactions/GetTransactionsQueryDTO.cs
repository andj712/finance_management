using finance_management.Models.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace finance_management.DTOs.GetTransactions
    {
        public class GetTransactionsQueryDTO 
        {
            [JsonProperty("transaction-kind")]
            public List<TransactionKindEnum>? TransactionKind { get; set; }

            [JsonProperty("start-date")]
            public DateTime? StartDate { get; set; }

            [JsonProperty("end-date")]
            public DateTime? EndDate { get; set; }

            [JsonProperty("page")]
            [Range(1, int.MaxValue)]
            public int Page { get; set; } = 1;

            [JsonProperty("page-size")]
            [Range(1, 100)]
            public int PageSize { get; set; } = 10;

            [JsonProperty("sort-by")]
            public string? SortBy { get; set; }

            [JsonProperty("sort-order")]
            public string SortOrder { get; set; } = "asc";


        }
}