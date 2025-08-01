using Newtonsoft.Json;

namespace finance_management.Validations.Errors
{
    public class ValidationResponse
    {
        [JsonProperty("errors")]
        public List<ValidationError> Errors { get; set; } = new();
    }
}
