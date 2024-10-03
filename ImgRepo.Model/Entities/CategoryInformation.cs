using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities
{
    /// <summary>
    /// 分類屬性的資訊
    /// </summary>
    public class CategoryInformation : IBasicEntityInformation
    {
        [Required]
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
