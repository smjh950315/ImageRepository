using ImgRepo.Data.Interface;
using System.ComponentModel.DataAnnotations;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Artist
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
        /// <seealso cref="Data.Enums.AttributeType"/>
        /// </summary>
        public long AttrType { get; set; }

        public long AttrId { get; set; }
    }
}
