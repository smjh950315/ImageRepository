#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace ImgRepo.Data
{
    /// <summary>
    /// 物件屬性DB模型的中繼資料
    /// </summary>
    public class AttributeMetaData
    {
        /// <summary>
        /// 物件屬性類型ID <see cref="Enums.AttributeType"/>"/>
        /// </summary>
        public long TypeId { get; set; }

        /// <summary>
        /// 物件屬性DB模型
        /// </summary>
        public Type ModelType { get; set; }
    }
}
