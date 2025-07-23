using Newtonsoft.Json;

namespace finance_management.Models
{
    public class SpendingAnalyticsInCategory
    {
        [JsonProperty("catcode")]
        public string CatCode { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}