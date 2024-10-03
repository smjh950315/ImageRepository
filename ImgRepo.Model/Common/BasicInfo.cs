using System.Text.Json.Serialization;

namespace ImgRepo.Model.Common
{
    /// <summary>
    /// 物件基本資訊WEB用模型
    /// </summary>
    public class BasicInfo
    {
        /// <summary>
        /// 物件資訊id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// 物件資訊名稱
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
