using ImgRepo.Data.Interface;
#pragma warning disable CS8618 // Non-nullable field is uninitialized.
namespace ImgRepo.Model.Entities.Image
{
    /// <summary>
    /// 圖片與屬性的關聯紀錄
    /// </summary>
    public class ImageRecord : IBasicEntityRecord
    {
        public long Id { get; set; }

        public long ObjectId { get; set; }

        /// <summary>
        /// <seealso cref="Enums.AttributeType"/>
        /// </summary>
        public long AttrType { get; set; }

        public long AttrId { get; set; }
    }
}
