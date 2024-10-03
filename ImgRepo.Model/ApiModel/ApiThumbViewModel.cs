using System.Text.Json.Serialization;

namespace ImgRepo.Model.ApiModel
{
    /// <summary>
    /// 附帶資訊的檔案模型
    /// </summary>
    public class ApiThumbViewModel : ApiFileModel
    {
        [JsonPropertyName("imageId")]
        public long ImageId { get; set; }

        [JsonPropertyName("imageName")]
        public string ImageName { get; set; }

        [JsonPropertyName("fileId")]
        public long FileId { get; set; }
    }
}
