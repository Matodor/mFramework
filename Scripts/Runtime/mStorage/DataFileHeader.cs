using System.Runtime.InteropServices;

namespace mFramework.Storage
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct DataFileHeader
    {
        [MarshalAs(UnmanagedType.I4)]
        public int ItemsNum;

        [MarshalAs(UnmanagedType.I8)]
        public long EncryptedSize;
    }
}