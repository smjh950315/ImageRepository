using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Image
{
    /// <summary>
    /// 圖片資訊
    /// </summary>
    public class ImageInformation : IBasicEntityInformation
    {
        [Required]
        public long Id { get; set; }

        [StringLength(512)]
        public string Name { get; set; }

        [StringLength(4096)]
        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Updated { get; set; }

        public long FileId { get; set; }

        public long? ArtistId { get; set; }
    }
}
