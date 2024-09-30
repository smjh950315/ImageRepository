using System.Text.Json.Serialization;

namespace ImgRepo.Model.ViewModel
{
    public class ImageThumbView
    {
        /// <summary>
        /// 圖像資訊id
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// 圖像資訊名稱
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 圖像資訊檔案id
        /// </summary>
        [JsonPropertyName("file_id")]
        public long FileId { get; set; }
    }
}
