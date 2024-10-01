using System.Text.Json.Serialization;

namespace ImgRepo.Model.ViewModel
{
    public class BasicInfo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
