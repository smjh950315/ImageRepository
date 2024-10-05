using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Image
{
    /// <summary>
    /// 圖片檔案資料
    /// </summary>
    public class ImageFileData
    {
        [Required]
        public long Id { get; set; }

        [StringLength(1024)]
        public string FileName { get; set; } = string.Empty;

        [StringLength(32)]
        public string Format { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public int Channel { get; set; }

        public int FileSize { get; set; }

        [StringLength(1024)]
        public string Uri { get; set; }
    }
}
