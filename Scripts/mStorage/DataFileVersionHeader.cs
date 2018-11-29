using System.Runtime.InteropServices;

namespace mFramework.Storage
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct DataFileVersionHeader
    {
        public string Magic => new string(MagicArray);

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] MagicArray;

        [MarshalAs(UnmanagedType.U1)]
        public byte Version;
    }
}