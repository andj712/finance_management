using Newtonsoft.Json;

namespace finance_management.Validations.Errors
{
    public class BusinessError
    {
        [JsonProperty("problem")]
        public string Problem { get; set; } = string.Empty;
        
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
       
        [JsonProperty("details")]
        public string Details { get; set; } = string.Empty;
    }
}
