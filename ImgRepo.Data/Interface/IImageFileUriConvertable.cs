namespace ImgRepo.Data.Interface
{
    /// <summary>
    /// 能夠還原出完整圖片檔案的Uri的物件
    /// </summary>
    public interface IImageFileUriConvertable
    {
        /// <summary>
        /// 圖片Id
        /// </summary>
        long ImageId { get; set; }
        /// <summary>
        /// 圖片檔案格式
        /// </summary>
        string Format { get; set; }
        /// <summary>
        /// 圖片檔案Uri
        /// </summary>
        string Uri { get; set; }
    }
}
