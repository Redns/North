namespace ImageBed.Common
{
    public class UnitNameGenerator
    {
        public const double FILESIZE_1KB = 1024.0;
        public const double FILESIZE_1MB = 1024 * 1024.0;
        public const double FILESIZE_1GB = 1024 * 1024 * 1024.0;


        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return (long)(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
        }


        /// <summary>
        /// 判断命名是否符合规范
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsLegalName(string name)
        {
            if (string.IsNullOrEmpty(name) || (name.Length > 255) || (name[0] == ' ') ||
               name.Contains('?') || name.Contains('/') || name.Contains('\\') ||
               name.Contains('<') || name.Contains('>') || name.Contains('*') ||
               name.Contains('|') || name.Contains(':'))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取文件拓展名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string? GetFileExtension(string filename)
        {
            return filename.Split('.').Last();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="len">文件大小（字节数）</param>
        /// <returns></returns>
        public static string RebuildFileSize(long len)
        {
            if (len < FILESIZE_1KB)
            {
                return $"{len}B";
            }
            else if (len < 1 * FILESIZE_1MB)
            {
                return $"{len / FILESIZE_1KB: #.##}KB";
            }
            else if (len < FILESIZE_1GB)
            {
                return $"{len / FILESIZE_1MB:#.##}MB";
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="len">字符串长度</param>
        /// <returns></returns>
        public static string GererateRandomString(int len)
        {
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks;
            Random random = new(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> len)));
            for (int i = 0; i < len; i++)
            {
                char ch;
                int num = random.Next();
                if ((num % 2) == 0)
                {
                    ch = (char)(0x30 + ((ushort)(num % 10)));
                }
                else
                {
                    ch = (char)(0x41 + ((ushort)(num % 0x1a)));
                }
                str += ch.ToString();
            }
            return str;
        }
    }
}
