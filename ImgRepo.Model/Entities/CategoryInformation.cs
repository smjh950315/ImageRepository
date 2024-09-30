using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    public class CategoryInformation : IBasicEntityInformation
    {
        [Required]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
