using Newtonsoft.Json;

namespace finance_management.Validations.Errors
{
    public class ValidationError
    {
        [JsonProperty("errors")]
        public List<Errors> Errors { get; set; }
    }
}
