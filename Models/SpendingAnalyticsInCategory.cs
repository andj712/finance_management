
using System.Text.Json.Serialization;

namespace finance_management.Models
{
    public class SpendingAnalyticsInCategory
    {
        [JsonPropertyName("catcode")]
        public string CatCode { get; set; }
       
        public double Amount { get; set; }
        
        public int Count { get; set; }
    }
}