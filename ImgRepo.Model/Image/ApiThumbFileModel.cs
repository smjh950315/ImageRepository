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
        /// <summary>
        /// 圖片Id
        /// </summary>
        [JsonPropertyName("imageId")]
        public long ImageId { get; set; }

        /// <summary>
        /// 圖片名稱
        /// </summary>
        [JsonPropertyName("imageName")]
        public string ImageName { get; set; }

        /// <summary>
        /// 檔案紀錄Id
        /// </summary>
        [JsonPropertyName("fileId")]
        public long FileId { get; set; }

        /// <summary>
        /// 實際檔案名稱(含副檔名)，不含路徑
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
    }
}
