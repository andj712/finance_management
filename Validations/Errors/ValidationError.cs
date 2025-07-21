using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace finance_management.Validations.Errors
{
    public class ValidationError
    {
        [Required]
        [JsonProperty("tag")]
        public string Tag { get; set; }
        [Required]
        [JsonProperty("error")]
        public string Error { get; set; }
        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
