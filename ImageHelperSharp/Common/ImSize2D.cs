using System.Runtime.InteropServices;

namespace ImageHelperSharp.Common
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImSize2D
    {
        [FieldOffset(0)]
        public int Width;

        [FieldOffset(4)]
        public int Height;

        public static bool operator ==(ImSize2D a, ImSize2D b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator !=(ImSize2D a, ImSize2D b)
        {
            return a.Width != b.Width || a.Height != b.Height;
        }
    }
}
