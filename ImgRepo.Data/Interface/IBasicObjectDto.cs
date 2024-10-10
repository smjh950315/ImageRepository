namespace ImgRepo.Data.Interface
{
    /// <summary>
    /// 基本物件資料傳輸物件
    /// </summary>
    public interface IBasicObjectDto
    {
        /// <summary>
        /// 物件名稱
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 物件描述
        /// </summary>
        string? Description { get; set; }
    }
}
