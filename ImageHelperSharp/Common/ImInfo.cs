using System.Runtime.InteropServices;

namespace ImageHelperSharp.Common
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImInfo
    {
        [FieldOffset(0)]
        internal ImSize2D size;

        [FieldOffset(8)]
        internal int channels;

        [FieldOffset(16)]
        internal IntPtr static_format_cstr;

        public readonly ImSize2D Size => this.size;
        public readonly int Channels => this.channels;
        public string Format
        {
            get
            {
                string? str = null;
                if (this.static_format_cstr != IntPtr.Zero)
                {
                    str = Marshal.PtrToStringUTF8(this.static_format_cstr);
                }
                return str ?? "unknown";
            }
        }
    }
}
