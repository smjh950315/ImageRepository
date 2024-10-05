using ImageHelperSharp.Common;
using ImageHelperSharp.Native;
using System.Diagnostics;

namespace ImageHelperSharp
{
    public class StbService
    {
        public static ImInfo GetImageFileInfo(byte[] data)
        {
            ImInfo imInfo = new ImInfo();
            unsafe
            {
                fixed (byte* dataPtr = data)
                {
                    int result = StbInterop.stb_getinfo(dataPtr, data.Length, ref imInfo);
                    if (result == -1)
                    {
                        throw new Exception("unknow format");
                    }
                }
            }
            return imInfo;
        }

        public static string GetFormat(byte[] data)
        {
            return GetImageFileInfo(data).Format;
        }

        public static byte[] Resize(byte[] data, ImSize2D size, int channels = 0, bool fix_ratio = true)
        {
            byte[] result;
            unsafe
            {
                void* _result;
                using (LifeTimeHandler lifeTime = new LifeTimeHandler(&_result, &StbInterop.stb_free))
                {
                    int resultLength;
                    fixed (byte* dataPtr = data)
                    {
                        resultLength = StbInterop.stb_resize(dataPtr, data.Length, ref size, channels, &_result, fix_ratio ? 1 : 0);
                    }
                    if (resultLength == -1)
                    {
                        throw new Exception("Failed to resize image");
                    }
                    result = new byte[resultLength];
                    fixed (byte* resultPtr = result)
                    {
                        Buffer.MemoryCopy(_result, resultPtr, resultLength, resultLength);
                    }
                }
                Debug.Assert(_result == null);
            }
            return result;
        }

        public static byte[] Resize(byte[] data, int width, int height, int channels = 0, bool fix_ratio = true)
        {
            return Resize(data, new ImSize2D { Width = width, Height = height }, channels, fix_ratio);
        }
    }
}
