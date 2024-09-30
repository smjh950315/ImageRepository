using System.Text.Json.Serialization;

namespace ImgRepo.Model.ViewModel
{
    public class ArtworkDetails : BasicDetails
    {
        /// <summary>
        /// 標籤清單
        /// </summary>
        [JsonPropertyName("tags")]
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// 分類清單
        /// </summary>
        [JsonPropertyName("categories")]
        public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();
    }
}
