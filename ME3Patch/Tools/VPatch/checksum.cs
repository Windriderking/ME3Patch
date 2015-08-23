using System;
using System.IO;
using System.Collections;

namespace VPatch4cs
{
    public static class checksum
    {
        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
        public static byte[] FileMD5(FileStream get_file,Hashtable Data,int i,bool adv)
        {
            if (i == -1||adv)
            {    //FileStream get_file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                System.Security.Cryptography.MD5CryptoServiceProvider get_md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash_byte = get_md5.ComputeHash(get_file);
                return hash_byte;
            }
            else
            {
               byte[] hash_byte= HexStringToByteArray( Data[Convert.ToString(i)].ToString());
               return hash_byte;
            }
                

        }
    }
public static class result
{
    public static int PATCH_SUCCESS =0;
    public static int PATCH_ERROR = 1;
    public static int PATCH_CORRUPT = 2;
    public static int PATCH_NOMATCH = 3;
    public static int PATCH_UPTODATE = 4;
    public static int FILE_ERR_PATCH = 5;
    public static int FILE_ERR_SOURCE = 6;
    public static int FILE_ERR_DEST = 7;
    public static int FILE_ERR_SAME = 8;
    public static int PATCH_UNSUPPORTED = 9;
}
}
