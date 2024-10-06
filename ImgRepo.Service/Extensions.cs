using Cyh.Net;

namespace ImgRepo.Service
{
    public static class Extensions
    {
        public static bool SaveObject(this IFileAccessService fileAccessService, string objectType, long objectId, string uri, string extName, byte[] data)
        {
            string uriPath = $"{objectType}/{objectId}/{uri}.{extName}";
            return fileAccessService.SaveFile(uriPath, data);
        }
        public static byte[] GetBytes(this Stream stream)
        {
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
        public static string[] SplitNoThrow(this string str, char separator)
        {
            return str.IsNullOrEmpty() ? Array.Empty<string>() : str.Split(separator);
        }
        public static string[] SplitNoThrow(this string str, string separator)
        {
            return str.IsNullOrEmpty() ? Array.Empty<string>() : str.Split(separator);
        }
    }
}
