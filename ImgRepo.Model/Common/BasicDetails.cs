using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Common
{
    /// <summary>
    /// 物件基本資訊WEB用模型
    /// </summary>
    public class BasicDetails : BasicInfo
    {
        /// <summary>
        /// 物件資訊描述
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
