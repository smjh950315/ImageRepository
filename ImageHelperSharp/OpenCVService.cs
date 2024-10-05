using ImageHelperSharp.Native;

namespace ImageHelperSharp
{
    public class OpenCVService
    {
        static IntPtr cv_get_matrix(byte[] bytes)
        {
            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    return OpenCVInterop.cv_get_matrix(ptr, bytes.Length);
                }
            }
        }

        static void cv_free_matrix_if_existing(ref IntPtr matPtr)
        {
            if (matPtr != IntPtr.Zero)
            {
                OpenCVInterop.cv_free_matrix(matPtr);
                matPtr = IntPtr.Zero;
            }
        }

        public static double GetImageDifferential(byte[] lhs, byte[] rhs)
        {
            byte[] ltemp = StbService.Resize(lhs, 256, 256, 0, false);
            byte[] rtemp = StbService.Resize(rhs, 256, 256, 0, false);

            nint lmat = cv_get_matrix(ltemp);
            nint rmat = cv_get_matrix(rtemp);

            if (lmat == IntPtr.Zero || rmat == IntPtr.Zero)
            {
                cv_free_matrix_if_existing(ref lmat);
                cv_free_matrix_if_existing(ref rmat);
                throw new Exception("Failed to get matrix");
            }

            return OpenCVInterop.cv_get_differential_by_mse(lmat, rmat);
        }
    }
}
