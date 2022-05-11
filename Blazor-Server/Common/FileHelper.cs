using ImageBed.Data.Entity;
using System.IO.Compression;

namespace ImageBed.Common
{
    public class FileHelper
    {
        public const double FILESIZE_1B  = 1.0;
        public const double FILESIZE_1KB = 1024.0;
        public const double FILESIZE_1MB = 1024 * 1024.0;
        public const double FILESIZE_1GB = 1024 * 1024 * 1024.0;


        /// <summary>
        /// 获取Linux时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }


        /// <summary>
        /// 判断命名是否符合规范(在Windows环境下)
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
        /// 获取文件拓展名(不带".")
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileExtension(string filename)
        {
            return filename.Split('.').Last();
        }


        /// <summary>
        /// 文件类型
        /// </summary>
        public enum FileType
        {
            IMAGE = 0,          // 图片
            COMPRESS,           // 压缩文件
            BIN,                // 二进制
            ILLEGAL             // 非法
        }


        /// <summary>
        /// 根据文件名后缀获取文件类型
        /// </summary>
        /// <param name="extension">文件后缀</param>
        /// <returns></returns>
        public static FileType GetFileType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return FileType.ILLEGAL;
            }
            else
            {
                extension = extension.ToLower();
                if (extension.Contains("jpg") ||
                    extension.Contains("png") ||
                    extension.Contains("jpeg") ||
                    extension.Contains("svg") ||
                    extension.Contains("bmp") ||
                    extension.Contains("gif") ||
                    extension.Contains("tiff") ||
                    extension.Contains("raw"))
                {
                    return FileType.IMAGE;
                }
                else if (extension.Contains("zip") ||
                        extension.Contains("rar") ||
                        extension.Contains("7z"))
                {
                    return FileType.COMPRESS;
                }
                else
                {
                    return FileType.BIN;
                }
            }
        }


        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="len">文件大小(单位:Byte)</param>
        /// <returns></returns>
        public static string RebuildFileSize(long len)
        {
            if (len < FILESIZE_1KB)
            {
                return $"{len} B";
            }
            else if (len < 1 * FILESIZE_1MB)
            {
                return $"{len / FILESIZE_1KB: #.##} KB";
            }
            else if (len < FILESIZE_1GB)
            {
                return $"{len / FILESIZE_1MB:#.##} MB";
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 解析文件大小字符串(B、KB、MB、GB)
        /// </summary>
        /// <param name="size">格式化后的文件大小</param>
        /// <returns>文件大小(以Byte计)</returns>
        public static double ParseFileSize(string size)
        {
            if (size.Contains("GB")) { return double.Parse(size[0..^3]) * FILESIZE_1GB; }
            else if (size.Contains("MB")) { return double.Parse(size[0..^3]) * FILESIZE_1MB; }
            else if (size.Contains("KB")) { return double.Parse(size[0..^3]) * FILESIZE_1KB; }
            else { return double.Parse(size[0..^3]) * FILESIZE_1B; }
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


        /// <summary>
        /// 重命名格式
        /// </summary>
        public enum RenameFormat
        {
            NONE = 0,               // 不重命名
            MD5,                    // MD5重命名
            TIME,                   // 时间重命名
            TIMESTAMP,              // 时间戳重命名
            RANDOM_STRING           // 随机字符串重命名
        }


        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="dir">文件夹</param>
        /// <param name="srcName">源文件名称</param>
        /// <param name="format">重命名规则</param>
        /// <returns></returns>
        public static string RenameFile(string dir, string srcName, RenameFormat format)
        {
            Directory.CreateDirectory(dir);

            if (format == RenameFormat.NONE) { return srcName; }
            string dstName = format switch
            {
                RenameFormat.MD5 => EncryptAndDecrypt.EncryptMD532(srcName),
                RenameFormat.TIME => DateTime.Now.ToLocalTime()
                                                 .ToString()
                                                 .Replace(":", "-")
                                                 .Replace("/", "-"),
                RenameFormat.TIMESTAMP => GetTimeStamp().ToString(),
                RenameFormat.RANDOM_STRING => GererateRandomString(8),
                _ => srcName,
            };
            dstName += $".{GetFileExtension(srcName)}";

            if ((format != RenameFormat.NONE) && File.Exists(dstName))
            {
                dstName = RenameFile(dir, dstName, format);
                return dstName;
            }
            else
            {
                return dstName;
            }
        }


        /// <summary>
        /// 链接格式
        /// </summary>
        public enum UrlFormat
        {
            Markdown = 0,
            Html,
            Url,
            UBB
        }


        /// <summary>
        /// 按指定形式构造图片URL
        /// </summary>
        /// <param name="format"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlBuild(UrlFormat format, string url)
        {
            return format switch
            {
                UrlFormat.Markdown => $"![{url.Split("/").Last()}]({url})",
                UrlFormat.Html => $"<img src=\"{url}\">",
                UrlFormat.UBB => $"[img]{url}[/img]",
                _ => url
            };
        }


        /// <summary>
        /// 多文件压缩
        /// </summary>
        /// <param name="srcFilepaths">源文件路径列表</param>
        public static async Task CompressMulti(IEnumerable<string> srcFilepaths, string zipFilepath)
        {
            try
            {
                string ExportTempDir = $"{GlobalValues.AppSetting.Data.Image.RootPath}/ExportTempDir";

                // 创建文件夹用于存储导出文件
                Directory.CreateDirectory(ExportTempDir);

                // 添加所有导入文件至文件夹
                foreach(string srcFilepath in srcFilepaths)
                {
                    var srcFileName = srcFilepath.Split("/").Last();
                    using(var fileReadStream = new FileStream(srcFilepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using(var fileWriteStream = File.Create($"{ExportTempDir}/{srcFileName}"))
                        {
                            await fileReadStream.CopyToAsync(fileWriteStream);
                            await fileWriteStream.FlushAsync();
                        }
                        _ = fileReadStream.FlushAsync();
                    }
                }

                // 压缩文件夹
                ZipFile.CreateFromDirectory(ExportTempDir, zipFilepath);

                // 删除文件夹
                Directory.Delete(ExportTempDir, true);
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Compress failed, {ex.Message}");
            }
        }


        /// <summary>
        /// 多文件压缩解压
        /// </summary>
        /// <param name="zipPath">压缩文件路径</param>
        /// <param name="targetPath">解压目录</param>
        public static void DeCompressMulti(string zipPath, string targetPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(zipPath, targetPath);
            }
            catch(Exception ex)
            {
                GlobalValues.Logger.Error($"Extract package failed, {ex.Message}");
            }
        }


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="imageReadStream">源图片流</param>
        /// <param name="imageName">图片原名称</param>
        /// <param name="imageDir">图片存储文件夹</param>
        /// <param name="owner">所有者</param>
        /// <returns></returns>
        public static async Task<ImageEntity> SaveImage(Stream imageReadStream, string imageName, string imageDir, string owner = "Admin")
        {
            // 格式化文件名
            string unitImageName = RenameFile(imageDir, imageName, GlobalValues.AppSetting?.Data.Image.RenameFormat ?? RenameFormat.NONE);
            string unitImageFullPath = $"{imageDir}/{unitImageName}";

            // 保存图片至磁盘
            using (FileStream imageWriteStream = File.Create(unitImageFullPath))
            {
                await imageReadStream.CopyToAsync(imageWriteStream);
                await imageWriteStream.FlushAsync();
            }

            // 生成缩略图
            using (var image = NetVips.Image.NewFromFile(unitImageFullPath))
            {
                using (var thumbnailImage = ImageHelper.ImageCut(image))
                {
                    if(thumbnailImage != null)
                    {
                        ImageHelper.ImageWaterMark(thumbnailImage, unitImageName, fontSize:25, leftEdge: 8, bottomEdge: 30)
                                   .WriteToFile($"{imageDir}/thumbnails_{unitImageName}");
                    }
                }

                // 构造图片信息
                return new(EncryptAndDecrypt.EncryptMD532(unitImageName),
                           unitImageName,
                           $"api/image/{unitImageName}",
                           $"{image.Width}×{image.Height}",
                           RebuildFileSize(imageReadStream.Length),
                           DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                           owner,
                           0);
            }
        }


        /// <summary>
        /// 导入图片
        /// </summary>
        /// <param name="zipFullPath">图片压缩包路径</param>
        /// <param name="importDir">图片导入路径</param>
        /// <returns></returns>
        public static async Task<List<ImageEntity>> ImportImages(string zipFullPath, string importDir, string owner)
        {
            List<ImageEntity> images = new();
            try
            {
                if(GetFileType(GetFileExtension(zipFullPath) ?? "") == FileType.COMPRESS)
                {
                    // 创建临时文件夹, 存储解压后的图片
                    // 这里不直接解压至 Images 文件夹下, 是为了方便导入图片信息至数据库 
                    string tempDir = $"{importDir}/Temp";
                    Directory.CreateDirectory(tempDir);

                    // 解压压缩包
                    // 录入解压出的所有图片信息, 并将其移动至 importDir 文件夹下
                    DeCompressMulti(zipFullPath, tempDir);
                    foreach (FileInfo imageInfo in new DirectoryInfo(tempDir).GetFiles())
                    {
                        var tempFileName = imageInfo.Name;
                        var tempFileFullpath = imageInfo.FullName;
                        if(GetFileType(GetFileExtension(tempFileName) ?? "") == FileType.IMAGE)
                        {
                            using (var imageReadStream = new FileStream(tempFileFullpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                images.Add(await SaveImage(imageReadStream, tempFileName, importDir, owner));
                                await imageReadStream.FlushAsync();
                            }
                        }
                    }

                    // 删除解压文件夹、压缩包文件
                    File.Delete(zipFullPath);
                    Directory.Delete(tempDir, true);
                }
                else
                {
                    GlobalValues.Logger.Error("Import package failed, can only import compress files");
                }
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Import package failed, {ex.Message}");
            }
            return images;
        }
    }
}
