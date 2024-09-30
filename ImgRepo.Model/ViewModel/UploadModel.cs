using System.Text.Json.Serialization;

namespace ImgRepo.Model.ViewModel
{
    public class UploadModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("descprition")]
        public string Description { get; set; }

        [JsonPropertyName("file")]
        public FileModel File { get; set; }
    }
}
