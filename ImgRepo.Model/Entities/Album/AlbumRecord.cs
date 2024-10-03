using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities.Album
{
    /// <summary>
    /// 相簿與屬性的關聯紀錄
    /// </summary>
    public class AlbumRecord : IBasicEntityRecord
    {
        [Required]
        public long Id { get; set; }
        public long ObjectId { get; set; }

        /// <summary>
        /// <seealso cref="Enums.AttributeType"/>
        /// </summary>
        public long AttrType { get; set; }
        public long AttrId { get; set; }
    }
}
