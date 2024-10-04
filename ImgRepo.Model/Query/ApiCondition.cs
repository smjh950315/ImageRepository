using Cyh.Net.Data.Predicate;
using Cyh.Net;
using System.Text.Json.Serialization;

namespace ImgRepo.Model.Query
{
    public class ApiCondition
    {
        [JsonPropertyName("name")]
        public string MemberName { get; set; } = string.Empty;

        [JsonPropertyName("operand")]
        public int LinkType { get; set; }

        [JsonPropertyName("operator")]
        public int CompareType { get; set; }

        [JsonPropertyName("constant")]
        public string? ConstantValue { get; set; }
    }
}
