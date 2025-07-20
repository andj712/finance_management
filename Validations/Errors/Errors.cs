using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace finance_management.Validations.Errors
{
    public class Errors
    {
        [Required]
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [Required]
        [JsonProperty("error")]
        public ErrorEnum Error { get; set; }
        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
