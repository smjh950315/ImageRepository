using ImgRepo.Data.Interface;

namespace ImgRepo.Data
{
    public static class Extensions
    {
        /// <summary>
        /// 取得完整的圖片檔案Uri
        /// </summary>
        /// <returns>檔案Uri</returns>
        public static string GetFullUri(this IImageFileUriConvertable imageFileUriConvertable)
        {
            return $"image/{imageFileUriConvertable.Uri}.{imageFileUriConvertable.Format}";
        }

        /// <summary>
        /// 取得縮圖的完整圖片檔案Uri
        /// </summary>
        /// <returns>檔案Uri</returns>
        public static string GetThumbFullUri(this IImageFileUriConvertable imageFileUriConvertable)
        {
            return $"image/{imageFileUriConvertable.Uri}_thumb.{imageFileUriConvertable.Format}";
        }
    }
}
