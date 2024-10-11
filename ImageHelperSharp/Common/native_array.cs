using System.Runtime.InteropServices;

namespace ImageHelperSharp.Common
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct NativeArray
    {
        public unsafe void* Data;
        public nuint Length;
        public nuint TypeSize;
    }
}
