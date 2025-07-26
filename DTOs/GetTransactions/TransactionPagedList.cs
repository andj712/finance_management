using finance_management.Models.Enums;
using Newtonsoft.Json;

namespace finance_management.DTOs.GetTransactions
{
    public class TransactionPagedList
    {
        [JsonProperty("total-count")]
        public int TotalCount { get; set; }

        [JsonProperty("page-size")]
        public int PageSize { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("total-pages")]
        public int TotalPages { get; set; }

        [JsonProperty("sort-order")]
        public SortOrderEnum SortOrder { get; set; }

        [JsonProperty("sort-by")]
        public string SortBy { get; set; }

        [JsonProperty("items")]
        public List<TransactionWithSplits> Items { get; set; } = new();
    }
}
