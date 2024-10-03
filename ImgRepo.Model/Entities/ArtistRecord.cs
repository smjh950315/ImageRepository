using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    public class ArtistRecord : IBasicEntityRecord
    {
        [Required]
        public long Id { get; set; }
        public long ObjectId { get; set; }
        /// <summary>
        /// 0=none, 1=tag, 2=category, 3=author
        /// </summary>
        public long AttrType { get; set; }
        public long AttrId { get; set; }
    }
}
