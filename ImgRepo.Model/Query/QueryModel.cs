using System.Text.Json.Serialization;

namespace ImgRepo.Model.Query
{
    /// <summary>
    /// 查詢使用的模型
    /// </summary>
    public class QueryModel
    {
        /// <summary>
        /// 條件集合
        /// </summary>
        [JsonPropertyName("conditions")]
        public ApiCondition[]? Conditions { get; set; }
    }
}
