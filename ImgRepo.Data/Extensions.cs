using ImgRepo.Data.Interface;

namespace ImgRepo.Data
{
    public static class Extensions
    {
        public static string GetFullUri(this IImageFileUriConvertable imageFileUriConvertable)
        {
            return $"image/{imageFileUriConvertable.Uri}.{imageFileUriConvertable.Format}";
        }
        public static string GetThumbFullUri(this IImageFileUriConvertable imageFileUriConvertable)
        {
            return $"image/{imageFileUriConvertable.Uri}_thumb.{imageFileUriConvertable.Format}";
        }
    }
}
