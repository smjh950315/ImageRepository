using ImgRepo.Model.Interface;

namespace ImgRepo.Model.Entities
{
    /// <summary>
    /// 圖片與屬性的關聯紀錄
    /// </summary>
    public class ImageRecord : IBasicEntityRecord
    {
        public long Id { get; set; }
        public long ObjectId { get; set; }
        /// <summary>
        /// 0=none, 1=tag, 2=category, 3=author
        /// </summary>
        public long AttrType { get; set; }
        public long AttrId { get; set; }
    }
}
