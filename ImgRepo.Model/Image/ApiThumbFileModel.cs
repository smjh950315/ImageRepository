using ImgRepo.Data.Interface;
using ImgRepo.Model.Common;
using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Image
{
    /// <summary>
    /// 附帶資訊的縮圖檔案模型
    /// </summary>
    public class ApiThumbFileModel : ApiFileModel, IImageFileUriConvertable
    {
        [JsonPropertyName("imageId")]
        public long ImageId { get; set; }

        [JsonPropertyName("imageName")]
        public string ImageName { get; set; }

        [JsonPropertyName("fileId")]
        public long FileId { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
