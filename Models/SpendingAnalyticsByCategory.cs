using Newtonsoft.Json;

namespace finance_management.Models
{
    public class SpendingAnalytics
    {
        [JsonProperty("groups")]
        public List<SpendingAnalyticsInCategory> Groups { get; set; }
    }

}
