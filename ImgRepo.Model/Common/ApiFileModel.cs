using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Common
{
    /// <summary>
    /// API使用的檔案模型
    /// </summary>
    public class ApiFileModel
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        /// <summary>
        /// 檔案格式
        /// </summary>
        [JsonPropertyName("format")]
        public string Format { get; set; }

        /// <summary>
        /// Base64編碼
        /// </summary>
        [JsonPropertyName("base64")]
        public string Base64 { get; set; }
    }
}
