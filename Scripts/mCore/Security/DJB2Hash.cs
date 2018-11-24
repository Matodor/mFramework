namespace mFramework.Core.Security
{
    public class DJB2Hash
    {
        public static ulong GetHash(string str)
        {
            var hash = 5381UL;
            for (int i = 0; i < str.Length; i++)
                hash = ((hash << 5) + hash) + str[i]; /* hash * 33 + c */

            return hash;
        }
    }
}
