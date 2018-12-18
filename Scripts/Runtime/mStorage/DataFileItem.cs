using System.Runtime.InteropServices;

namespace mFramework.Storage
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct DataFileItem
    {
        [MarshalAs(UnmanagedType.U1)]
        public byte KeySize;

        [MarshalAs(UnmanagedType.I4)]
        public int DataSize;
    }
}