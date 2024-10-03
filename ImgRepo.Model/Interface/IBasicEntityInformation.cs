namespace ImgRepo.Model.Interface
{
    /// <summary>
    /// image, author, album 物件的基本資訊介面
    /// </summary>
    public interface IBasicEntityInformation
    {
        /// <summary>
        /// 流水號主鍵
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// 物件名稱
        /// </summary>
        string Name { get; set; }

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
