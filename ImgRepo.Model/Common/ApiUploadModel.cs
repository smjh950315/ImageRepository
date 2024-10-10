using ImgRepo.Data.Interface;
using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Common
{
    /// <summary>
    /// API使用的上傳模型
    /// </summary>
    public class ApiUploadModel : IBasicUploadModel
    {
        /// <summary>
        /// 物件名稱
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// 物件描述
        /// </summary>
        [JsonPropertyName("descprition")]
        public string? Description { get; set; }

        /// <summary>
        /// 物件標籤屬性(以逗號分隔)
        /// </summary>
        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// 物件分類屬性(以逗號分隔)
        /// </summary>
        [JsonPropertyName("categories")]
        public string Categories { get; set; }

        /// <summary>
        /// 物件檔案
        /// </summary>
        [JsonPropertyName("file")]
        public ApiFileModel File { get; set; }
    }
}
