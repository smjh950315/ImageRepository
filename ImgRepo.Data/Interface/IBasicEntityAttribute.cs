namespace ImgRepo.Data.Interface
{
    public interface IBasicEntityAttribute
    {
        /// <summary>
        /// 流水號主鍵
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// 屬性值
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 物件描述
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// 物件建立時間
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// 物件更新時間
        /// </summary>
        DateTime? Updated { get; set; }
    }
}
