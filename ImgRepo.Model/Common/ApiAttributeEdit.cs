using System.Text.Json.Serialization;

namespace ImgRepo.Model.Common
{
    public class ApiAttributeEdit
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
