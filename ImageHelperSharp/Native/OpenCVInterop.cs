using System.Runtime.InteropServices;

namespace ImageHelperSharp.Native
{
    internal unsafe class OpenCVInterop : SharedConstant
    {
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_matrix")]
        internal static extern void* cv_get_matrix(void* data, int length);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_free_matrix")]
        internal static extern void cv_free_matrix(void* matPtr);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_differential_by_mse")]
        internal static extern int cv_get_differential_by_mse(void* lmat, void* rmat, ref double result);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_ssim_similarity")]
        internal static extern int cv_get_ssim_similarity(void* lmat, void* rmat, ref double result);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_differential_bfmatch")]
        internal static extern void* cv_get_differential_bfmatch(void* lmat, void* rmat);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_encode_png_to_c_lang_malloc")]
        internal static extern int cv_encode_png_to_c_lang_malloc(void* pmat, void** resultPtr);
    }
}
