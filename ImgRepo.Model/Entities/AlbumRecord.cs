using ImgRepo.Model.Interface;
using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    public class AlbumRecord : IBasicEntityRecord
    {
        [Required]
        public long Id { get; set; }
        public long ObjectId { get; set; }

        /// <summary>
        /// <seealso cref="ImgRepo.Model.Enums.AttributeType"/>
        /// </summary>
        public long AttrType { get; set; }
        public long AttrId { get; set; }
    }
}
