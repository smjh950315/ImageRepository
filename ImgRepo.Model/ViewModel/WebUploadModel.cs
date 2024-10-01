using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.ViewModel
{
    public class WebUploadModel
    {
        public string ImageName { get; set; }
        public string Description { get; set; }

        public string FileName { get; set; }

        public byte[] FileData { get; set; }
        public string[] Tags { get; set; }
        public string[] Categories { get; set; }
    }
}
