using System.Text.Json.Serialization;

namespace ImgRepo.Model.Common
{
    /// <summary>
    /// 用來編輯物件屬性的API模型
    /// </summary>
    public class ApiAttributeEdit
    {
        /// <summary>
        /// 物件ID
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// 屬性名稱
        /// </summary>
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
