using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    public class ImageFileData
    {
        [Required]
        public long Id { get; set; }

        public byte[]? Data { get; set; }
    }
}
