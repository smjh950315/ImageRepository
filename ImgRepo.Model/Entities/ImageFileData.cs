using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    public class ImageFileData
    {
        [Required]
        public long Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Format { get; set; } = string.Empty;

        /// <summary>
        /// 預覽圖像資料(小圖 256x256)
        /// </summary>
        public byte[]? Thumbnail { get; set; }

        public byte[]? Data { get; set; }
    }
}
