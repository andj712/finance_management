using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace finance_management.Models
{
    public class SingleCategorySplit
    {
        [Required]
        [JsonProperty("catcode")]
        public string CatCode { get; set; }
        [Required]
        [JsonProperty("amount")]
        public double Amount { get; set; }
    }
}
