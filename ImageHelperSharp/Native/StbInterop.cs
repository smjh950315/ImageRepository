using ImageHelperSharp.Common;
using System.Runtime.InteropServices;

namespace ImageHelperSharp.Native
{
    internal class StbInterop : SharedConstant
    {
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stb_resize")]
        internal static unsafe extern int stb_resize(void* data, int length, ref ImSize2D size, int channels, void** result, int fix_ratio);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stb_free")]
        internal static unsafe extern void stb_free(void* data);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "stb_getinfo")]
        internal static unsafe extern int stb_getinfo(void* data, int length, ref ImInfo imInfo);
    }
}
