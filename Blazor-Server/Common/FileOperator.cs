using ImageBed.Data.Entity;
using System.IO.Compression;
using static ImageBed.Common.UnitNameGenerator;

namespace ImageBed.Common
{
    public class FileOperator
    {
        /// <summary>
        /// 多文件压缩
        /// </summary>
        /// <param name="srcFilepaths">源文件路径列表</param>
        public static async Task CompressMulti(IEnumerable<string> srcFilepaths, string zipFilepath)
        {
            GlobalValues.Logger.Info($"Compressing {srcFilepaths.Count()} files...");

            try
            {
                string ExportTempDir = $"{GlobalValues.appSetting.Data.Resources.Images.Path}/ExportTempDir";
                if (Directory.Exists(ExportTempDir))
                {
                    Directory.Delete(ExportTempDir, true);
                }
                Directory.CreateDirectory(ExportTempDir);

                foreach(string srcFilepath in srcFilepaths)
                {
                    var srcFileName = srcFilepath.Split("/").Last();
                    await SaveFile(new FileStream(srcFilepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), $"{ExportTempDir}/{srcFileName}");
                }
                ZipFile.CreateFromDirectory(ExportTempDir, zipFilepath);
                Directory.Delete(ExportTempDir, true);
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Compress images failed, {ex.Message}");
            }
                
            GlobalValues.Logger.Info("Compress finished");
        }


        /// <summary>
        /// 多文件压缩解压
        /// </summary>
        /// <param name="zipPath">压缩文件路径</param>
        /// <param name="targetPath">解压目录</param>
        public static void DeCompressMulti(string zipPath, string targetPath)
        {
            GlobalValues.Logger.Info("Extracting package...");

            try
            {
                ZipFile.ExtractToDirectory(zipPath, targetPath);
            }
            catch(Exception ex)
            {
                GlobalValues.Logger.Error($"Extract package failed, {ex.Message}");
            }

            GlobalValues.Logger.Info("Extract package finished");
        }


        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileReader">文件输入流</param>
        /// <param name="dstPath">文件存储路径</param>
        /// <returns></returns>
        public static async Task SaveFile(Stream fileReader, string dstPath)
        {
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }

            using (FileStream fileWriter = File.Create(dstPath))
            {
                await fileReader.CopyToAsync(fileWriter);
                await fileReader.FlushAsync();
                fileReader.Dispose();
            }
        }


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="imageReader">待写入的图片流</param>
        /// <param name="imageName">图片原名称</param>
        /// <param name="imageDir">图片存储文件夹</param>
        /// <returns></returns>
        public static async Task<ImageEntity> SaveImage(Stream imageReader, string imageName, string imageDir)
        {
            // 格式化文件名
            GlobalValues.Logger.Info($"Rename image... Current rename format is {GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat}");
            
            RenameFormat renameFormat = GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat ?? RenameFormat.MD5;
            string unitImageName = RenameFile(imageDir, imageName, renameFormat);
            string unitImageFullPath = $"{imageDir}/{unitImageName}";

            // 检查是否命名冲突
            if ((renameFormat == RenameFormat.NONE) && File.Exists(unitImageFullPath))
            {
                File.Delete(unitImageFullPath);
            }

            // 保存图片
            using (FileStream imageWriter = File.Create(unitImageFullPath))
            {
                await imageReader.CopyToAsync(imageWriter);
                await imageWriter.FlushAsync();
                await imageReader.FlushAsync();
                await imageReader.DisposeAsync();
            }

            // 生成缩略图
            var imageInfo = NetVips.Image.NewFromFile(unitImageFullPath);
            imageInfo.ThumbnailImage(180, 135).WriteToFile($"{imageDir}/thumbnails_{unitImageName}");

            // 录入数据库
            var fileInfo = new FileInfo(unitImageFullPath);
            ImageEntity image = new()
            {
                Id = EncryptAndDecrypt.Encrypt_MD5(unitImageName),
                Name = unitImageName,
                Url = $"api/image/{unitImageName}",
                Dpi = $"{imageInfo.Width}*{imageInfo.Height}",
                Size = RebuildFileSize(fileInfo.Length),
                UploadTime = DateTime.Now.ToString(),
                Owner = "Admin"
            };
            imageInfo.Close();
            imageInfo.Dispose();

            return image;
        }


        /// <summary>
        /// 导入图片
        /// </summary>
        /// <param name="zipFullPath">图片压缩包路径</param>
        /// <param name="importDir">图片导入路径</param>
        /// <returns></returns>
        public static async Task<List<ImageEntity>> ImportImages(string zipFullPath, string importDir)
        {
            List<ImageEntity> images = new();
            try
            {
                if(GetFileType(GetFileExtension(zipFullPath) ?? "") == FileType.COMPRESS)
                {
                    // 创建临时文件夹
                    string tempDir = $"{importDir}/Temp";
                    if (!Directory.Exists(importDir))
                    {
                        Directory.CreateDirectory(importDir);
                        Directory.CreateDirectory(tempDir);
                    }
                    else if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }

                    // 解压压缩包
                    // 录入解压出的所有图片信息, 并将其移动至 importDir 文件夹下
                    DeCompressMulti(zipFullPath, tempDir);
                    foreach (FileInfo imageInfo in new DirectoryInfo(tempDir).GetFiles())
                    {
                        var tempFileName = imageInfo.Name;
                        var tempFileFullpath = imageInfo.FullName;
                        if(GetFileType(GetFileExtension(tempFileName) ?? "") == FileType.IMAGE)
                        {
                            using (var imageReader = new FileStream(tempFileFullpath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                images.Add(await SaveImage(imageReader, tempFileName, importDir));
                            }
                        }
                    }
                    Directory.Delete(tempDir, true);
                    File.Delete(zipFullPath);
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
