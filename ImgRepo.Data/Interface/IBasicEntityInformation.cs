namespace ImgRepo.Data.Interface
{
    /// <summary>
    /// image, author, album 物件的基本資訊介面
    /// </summary>
    public interface IBasicEntityInformation : IBasicObjectDto
    {
        /// <summary>
        /// 流水號主鍵
        /// </summary>
        long Id { get; set; }

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
