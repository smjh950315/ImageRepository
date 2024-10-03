namespace ImgRepo.Model.Entities.Bindings
{
    /// <summary>
    /// 藝術家與圖片的關聯紀錄
    /// </summary>
    public class ArtistImageBinding
    {
        public long Id { get; set; }

        /// <summary>
        /// ArtistId
        /// </summary>
        public long ArtistId { get; set; }

        /// <summary>
        /// ImageId
        /// </summary>
        public long ImageId { get; set; }
    }
}
