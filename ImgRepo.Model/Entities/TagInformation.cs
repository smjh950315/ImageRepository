using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities
{
    public class TagInformation : IBasicEntityInformation
    {
        [Required]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = String.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
    }
}
