using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.ApiModel
{
    public class ApiFileModel
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("format")]
        public string Format { get; set; }

        [JsonPropertyName("base64")]
        public string Base64 { get; set; }
    }
}
