using System.ComponentModel.DataAnnotations;

namespace ImgRepo.Model.Entities
{
    public class ArtistAdditionalRecord
    {
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// 藝術家Id
        /// </summary>
        public long ArtistId { get; set; }
        /// <summary>
        /// 額外資訊類型
        /// <seealso cref="ImgRepo.Model.Enums.ArtistAdditionalType"/>
        /// </summary>
        public long AdditionalType { get; set; }
        /// <summary>
        /// 額外資訊
        /// </summary>
        public string? AdditionalData { get; set; }
    }
}
