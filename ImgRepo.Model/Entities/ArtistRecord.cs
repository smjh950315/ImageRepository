using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    /// <summary>
    /// 藝術家與屬性的關聯紀錄
    /// </summary>
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
