using System.Text.Json.Serialization;

namespace ImgRepo.Model.ApiModel
{
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
