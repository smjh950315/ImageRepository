namespace ImgRepo.Model.Interface
{
    /// <summary>
    /// 基本物件上傳模型
    /// </summary>
    public interface IBasicUploadModel
    {
        /// <summary>
        /// 物件名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 物件描述
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 物件標籤屬性(以逗號分隔)
        /// </summary>
        string Tags { get; set; }

        /// <summary>
        /// 物件分類屬性(以逗號分隔)
        /// </summary>
        string Categories { get; set; }
    }
}
