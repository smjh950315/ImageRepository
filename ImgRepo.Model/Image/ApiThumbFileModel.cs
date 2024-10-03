using System.Text.Json.Serialization;
using ImgRepo.Model.Common;

namespace ImgRepo.Model.Image
{
    /// <summary>
    /// 附帶資訊的縮圖檔案模型
    /// </summary>
    public class ApiThumbFileModel : ApiFileModel
    {
        [JsonPropertyName("imageId")]
        public long ImageId { get; set; }

        [JsonPropertyName("imageName")]
        public string ImageName { get; set; }

        [JsonPropertyName("fileId")]
        public long FileId { get; set; }
    }
}
