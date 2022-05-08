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
                var imageConfig = GlobalValues.appSetting.Data.Resources.Images;
                string ExportTempDir = $"{imageConfig.Path}/ExportTempDir";

                // 创建文件夹用于存储导出文件
                Directory.CreateDirectory(ExportTempDir);

                // 添加所有导入文件至文件夹
                foreach(string srcFilepath in srcFilepaths)
                {
                    var srcFileName = srcFilepath.Split("/").Last();
                    using(var fileReadStream = new FileStream(srcFilepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        await SaveFile(fileReadStream, $"{ExportTempDir}/{srcFileName}");
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
            using (FileStream fileWriter = File.Create(dstPath))
            {
                await fileReader.CopyToAsync(fileWriter);
                await fileWriter.FlushAsync();
            }
        }


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="imageReadStream">源图片流</param>
        /// <param name="imageName">图片原名称</param>
        /// <param name="imageDir">图片存储文件夹</param>
        /// <returns></returns>
        public static async Task<ImageEntity> SaveImage(Stream imageReadStream, string imageName, string imageDir)
        {
            GlobalValues.Logger.Info($"Rename image... Current rename format is {GlobalValues.appSetting?.Data?.Resources?.Images?.RenameFormat}");

            // 格式化文件名
            var imageConfig = GlobalValues.appSetting?.Data?.Resources?.Images;
            string unitImageName = RenameFile(imageDir, imageName, imageConfig.RenameFormat);
            string unitImageFullPath = $"{imageDir}/{unitImageName}";

            // 保存图片至磁盘
            using (FileStream imageWriteStream = File.Create(unitImageFullPath))
            {
                await imageReadStream.CopyToAsync(imageWriteStream);
                await imageWriteStream.FlushAsync();
            }

            using (var image = NetVips.Image.NewFromFile(unitImageFullPath))
            {
                // 生成缩略图
                using (var thumbnailImage = ImageHelper.ImageCut(image))
                {
                    if(thumbnailImage != null)
                    {
                        ImageHelper.ImageWaterMark(thumbnailImage, unitImageName, fontSize:25, leftEdge: 8, bottomEdge: 30)
                                   .WriteToFile($"{imageDir}/thumbnails_{unitImageName}");
                    }
                }

                // 构造图片信息
                return new()
                {
                    Id = EncryptAndDecrypt.MD5Encrypt32(unitImageName),
                    Name = unitImageName,
                    Url = $"api/image/{unitImageName}",
                    Dpi = $"{image.Width}×{image.Height}",
                    Size = RebuildFileSize(imageReadStream.Length),
                    UploadTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    Owner = "Admin"
                };
            }
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
                                images.Add(await SaveImage(imageReadStream, tempFileName, importDir));
                                _ = imageReadStream.FlushAsync();
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
