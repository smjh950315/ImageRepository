using System.Runtime.InteropServices;

namespace ImageHelperSharp.Native
{
    internal class OpenCVInterop : SharedConstant
    {
        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_matrix")]
        internal static unsafe extern IntPtr cv_get_matrix(byte* data, int length);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_free_matrix")]
        internal static unsafe extern void cv_free_matrix(IntPtr matPtr);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_differential_by_mse")]
        internal static unsafe extern double cv_get_differential_by_mse(IntPtr lmat, IntPtr rmat);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_differential_by_ssim")]
        internal static unsafe extern double cv_get_differential_by_ssim(IntPtr lmat, IntPtr rmat);

        [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "cv_get_differential_bfmatch")]
        internal static unsafe extern IntPtr cv_get_differential_bfmatch(IntPtr lmat, IntPtr rmat);

        internal static IntPtr cv_get_matrix(byte[] bytes)
        {
            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    return cv_get_matrix(ptr, bytes.Length);
                }
            }
        }

        internal static void cv_free_matrix_if_existing(ref IntPtr matPtr)
        {
            if (matPtr != IntPtr.Zero)
            {
                cv_free_matrix(matPtr);
                matPtr = IntPtr.Zero;
            }
        }
    }
}
