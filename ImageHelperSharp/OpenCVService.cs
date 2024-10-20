using ImageHelperSharp.Common;
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
                    return unchecked((IntPtr)OpenCVInterop.cv_get_matrix(ptr, bytes.Length));
                }
            }
        }

        static void cv_free_matrix_if_existing(ref IntPtr matPtr)
        {
            if (matPtr != IntPtr.Zero)
            {
                unsafe { OpenCVInterop.cv_free_matrix((void*)matPtr); }
                matPtr = IntPtr.Zero;
            }
        }

        static byte[]? cv_mat_to_png_bytes(IntPtr matPtr)
        {
            if (matPtr == IntPtr.Zero) return null;

            unsafe
            {
                void* resultPtr = null;
                using (LifeTimeHandler lifeTimeHandler = new LifeTimeHandler(&resultPtr, &SharedConstant.c_lang_free))
                {
                    int byteLength = OpenCVInterop.cv_encode_png_to_c_lang_malloc((void*)matPtr, &resultPtr);
                    if (byteLength == 0) return null;
                    byte[] output = new byte[byteLength];

                    fixed (byte* outputPtr = output)
                    {
                        Buffer.MemoryCopy(resultPtr, outputPtr, byteLength, byteLength);
                    }
                    return output;
                }
            }
        }

        public static double GetImageDifferential(byte[] lhs, byte[] rhs)
        {
            byte[] ltemp = StbService.Resize(lhs, 256, 256, 0, false);
            byte[] rtemp = StbService.Resize(rhs, 256, 256, 0, false);

            IntPtr lmat = cv_get_matrix(ltemp);
            IntPtr rmat = cv_get_matrix(rtemp);

            if (lmat == IntPtr.Zero || rmat == IntPtr.Zero)
            {
                cv_free_matrix_if_existing(ref lmat);
                cv_free_matrix_if_existing(ref rmat);
                throw new Exception("Failed to get matrix");
            }

            int success;
            double differentialValue = -1;
            try
            {
                unsafe { success = OpenCVInterop.cv_get_differential_by_mse((void*)lmat, (void*)rmat, ref differentialValue); }
            }
            catch
            {
                success = -1;
            }
            finally
            {
                cv_free_matrix_if_existing(ref lmat);
                cv_free_matrix_if_existing(ref rmat);
            }

            return success == -1 ? throw new Exception("Failed to get differential value") : differentialValue;
        }

        public static double GetSSIMSimilarity(byte[] lhs, byte[] rhs)
        {
            byte[] ltemp = StbService.Resize(lhs, 256, 256, 0, false);
            byte[] rtemp = StbService.Resize(rhs, 256, 256, 0, false);
            IntPtr lmat = cv_get_matrix(ltemp);
            IntPtr rmat = cv_get_matrix(rtemp);
            if (lmat == IntPtr.Zero || rmat == IntPtr.Zero)
            {
                cv_free_matrix_if_existing(ref lmat);
                cv_free_matrix_if_existing(ref rmat);
                throw new Exception("Failed to get matrix");
            }

            int success;
            double differentialValue = -1;
            try
            {
                unsafe { success = OpenCVInterop.cv_get_ssim_similarity((void*)lmat, (void*)rmat, ref differentialValue); }
            }
            catch
            {
                success = -1;
            }
            finally
            {
                cv_free_matrix_if_existing(ref lmat);
                cv_free_matrix_if_existing(ref rmat);
            }

            return success == -1 ? throw new Exception("Failed to get ssim similarity") : differentialValue;
        }

        public static byte[]? GetPatternMatchImage(byte[] limage, byte[] rimage)
        {
            IntPtr lmat = cv_get_matrix(limage);
            IntPtr rmat = cv_get_matrix(rimage);
            if (lmat == IntPtr.Zero || rmat == IntPtr.Zero)
            {
                cv_free_matrix_if_existing(ref lmat);
                cv_free_matrix_if_existing(ref rmat);
                throw new Exception("Failed to get matrix");
            }

            byte[]? result;

            unsafe
            {
                void* matPtr = null;
                using (LifeTimeHandler lifeTimeHandler = new LifeTimeHandler(&matPtr, &OpenCVInterop.cv_free_matrix))
                {
                    matPtr = OpenCVInterop.cv_get_differential_bfmatch((void*)lmat, (void*)rmat);
                    result = matPtr != null ? cv_mat_to_png_bytes((IntPtr)matPtr) : null;
                }
            }

            cv_free_matrix_if_existing(ref lmat);
            cv_free_matrix_if_existing(ref rmat);

            return result;
        }
    }
}
