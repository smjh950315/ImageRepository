using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities
{
    /// <summary>
    /// 標籤的資訊
    /// </summary>
    public class TagInformation : IBasicEntityInformation
    {
        [Required]
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Updated { get; set; }
    }
}
