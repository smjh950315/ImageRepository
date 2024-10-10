using System.Text.Json.Serialization;

namespace ImgRepo.Model.Query
{
    /// <summary>
    /// 查詢條件
    /// </summary>
    public class ApiCondition
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("operand")]
        public int LinkType { get; set; }

        [JsonPropertyName("operator")]
        public int CompareType { get; set; }

        [JsonPropertyName("constant")]
        public string? ConstantValue { get; set; }
    }
}
