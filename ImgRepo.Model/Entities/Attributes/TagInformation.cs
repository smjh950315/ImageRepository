using ImgRepo.Data.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Attributes
{
    /// <summary>
    /// 標籤的資訊
    /// </summary>
    public class TagInformation : IBasicEntityAttribute
    {
        [Required]
        public long Id { get; set; }

        [StringLength(512)]
        public string Value { get; set; }

        [StringLength(4096)]
        public string? Description { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? Updated { get; set; }
    }
}
