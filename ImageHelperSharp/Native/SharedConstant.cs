using System.Runtime.InteropServices;

namespace ImageHelperSharp.Native
{
    internal unsafe class SharedConstant
    {
        public const string DllPath = "ExternLib/ImageHelper.dll";

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "c_lang_malloc")]
        internal static extern void* c_lang_malloc(nuint size);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "c_lang_free")]
        internal static extern void c_lang_free(void* _block);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "c_lang_realloc")]
        internal static extern void* c_lang_realloc(void* _block, nuint size);
    }
}
