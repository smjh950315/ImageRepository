namespace ImgRepo.Model.Entities.Bindings
{
    /// <summary>
    /// 相簿與圖片的關聯紀錄
    /// </summary>
    public class AlbumImageBinding
    {
        public long Id { get; set; }

        /// <summary>
        /// Order of the image in the album
        /// </summary>
        public long Order { get; set; }

        /// <summary>
        /// AlbumId
        /// </summary>
        public long AlbumId { get; set; }

        /// <summary>
        /// ImageId
        /// </summary>
        public long ImageId { get; set; }
    }
}
