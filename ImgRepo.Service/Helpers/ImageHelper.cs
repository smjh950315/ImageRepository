using System.Runtime.InteropServices;

namespace ImgRepo.Service.Helpers
{
    public static class ImageHelper
    {
        const string DllPath = "ExternLib/ImageHelper.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stb_resize")]
        static unsafe extern int stb_resize(void* data, int length, int width, int height, void** result, int fix_ratio);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stb_free")]
        static unsafe extern void stb_free(void* data);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stb_getinfo")]
        static unsafe extern int stb_getinfo(void* data, int length, int* x, int* y, int* comp, void* fmtStr);

        public static ImageFileInfo GetImageFileInfo(byte[] data)
        {
            int x, y, comp;
            string fmt;
            unsafe
            {
                sbyte* fmtStr = stackalloc sbyte[5];
                fixed (byte* dataPtr = data)
                {
                    int result = stb_getinfo(dataPtr, data.Length, &x, &y, &comp, fmtStr);
                    if (result == -1)
                    {
                        x = y = comp = 0;
                        fmt = "unknow";
                    }
                    else
                    {
                        fmt = new string(fmtStr);
                    }
                }
            }
            return new ImageFileInfo
            {
                Width = x,
                Height = y,
                Comp = comp,
                Format = fmt
            };
        }

        public static string GetFormat(byte[] data)
        {
            return GetImageFileInfo(data).Format;
        }

        public static byte[] Resize(byte[] data, int width, int height)
        {
            byte[] result;
            unsafe
            {
                int resultLength;
                void* _result;
                fixed (byte* dataPtr = data)
                {
                    resultLength = stb_resize(dataPtr, data.Length, width, height, &_result, 1);
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
                stb_free(_result);
            }
            return result;
        }
    }
}
