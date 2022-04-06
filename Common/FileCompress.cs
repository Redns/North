using System.IO.Compression;

namespace ImageBed.Common
{
    public class FileCompress
    {
        /// <summary>
        /// 自定义多文件压缩（生成的压缩包和第三方的压缩文件解压不兼容）
        /// </summary>
        /// <param name="sourceFileList">文件列表</param>
        /// <param name="saveFullPath">压缩包全路径</param>
        public static void CompressMulti(string[] sourceFileList, string saveFullPath)
        {
            MemoryStream ms = new();
            foreach (string filePath in sourceFileList)
            {
                if (File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                    byte[] sizeBytes = BitConverter.GetBytes(fileNameBytes.Length);
                    ms.Write(sizeBytes, 0, sizeBytes.Length);
                    ms.Write(fileNameBytes, 0, fileNameBytes.Length);
                    byte[] fileContentBytes = File.ReadAllBytes(filePath);
                    ms.Write(BitConverter.GetBytes(fileContentBytes.Length), 0, 4);
                    ms.Write(fileContentBytes, 0, fileContentBytes.Length);
                }
            }
            ms.Flush();
            ms.Position = 0;
            using (FileStream zipFileStream = File.Create(saveFullPath))
            {
                using GZipStream zipStream = new(zipFileStream, CompressionMode.Compress);
                ms.Position = 0;
                ms.CopyTo(zipStream);
            }
            ms.Close();
        }


        /// <summary>
        /// 多文件压缩解压
        /// </summary>
        /// <param name="zipPath">压缩文件路径</param>
        /// <param name="targetPath">解压目录</param>
        public static void DeCompressMulti(string zipPath, string targetPath)
        {
            byte[] fileSize = new byte[4];
            if (File.Exists(zipPath))
            {
                using FileStream fStream = File.Open(zipPath, FileMode.Open);
                using MemoryStream ms = new MemoryStream();
                using (GZipStream zipStream = new(fStream, CompressionMode.Decompress))
                {
                    zipStream.CopyTo(ms);
                }
                ms.Position = 0;
                while (ms.Position != ms.Length)
                {
                    ms.Read(fileSize, 0, fileSize.Length);
                    int fileNameLength = BitConverter.ToInt32(fileSize, 0);
                    byte[] fileNameBytes = new byte[fileNameLength];
                    ms.Read(fileNameBytes, 0, fileNameBytes.Length);
                    string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);
                    string fileFulleName = targetPath + fileName;
                    ms.Read(fileSize, 0, 4);
                    int fileContentLength = BitConverter.ToInt32(fileSize, 0);
                    byte[] fileContentBytes = new byte[fileContentLength];
                    ms.Read(fileContentBytes, 0, fileContentBytes.Length);
                    using FileStream childFileStream = File.Create(fileFulleName);
                    childFileStream.Write(fileContentBytes, 0, fileContentBytes.Length);
                }
            }
        }
    }
}
