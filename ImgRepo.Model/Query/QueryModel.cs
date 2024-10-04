using System.Text.Json.Serialization;

namespace ImgRepo.Model.Query
{
    public class QueryModel
    {
        [JsonPropertyName("conditions")]
        public ApiCondition[]? Conditions { get; set; }
    }
}
