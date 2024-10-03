using ImgRepo.Model.Interface;
using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.ApiModel
{
    public class ApiUploadModel : IBasicUploadModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("descprition")]
        public string Description { get; set; }

        [JsonPropertyName("file")]
        public ApiFileModel File { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("categories")]
        public string Categories { get; set; }
    }
}
