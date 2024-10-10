namespace ImgRepo.Model.Image
{
    /// <summary>
    /// 圖片內容資料
    /// </summary>
    public class ImageContentData
    {
        /// <summary>
        /// 圖片GUID，作為檔名使用
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// 圖片資料
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 圖片副檔名
        /// </summary>
        public string ExtName { get; set; }
    }
}
