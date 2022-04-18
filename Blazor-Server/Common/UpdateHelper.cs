using ImageBed.Data.Entity;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;

namespace ImageBed.Common
{
    public class UpdateHelper
    {
        public const string tagGetUrl = "https://api.github.com/repos/Redns/ImageBed/tags?per_page=10&page=1";
        public const string releaseDownloadBaseUrl = "https://github.com/Redns/ImageBed/releases/download";


        /// <summary>
        /// 获取最新版本号
        /// </summary>
        /// <param name="checkUrl">数据源</param>
        /// <returns>获取成功返回最新版本号, 否则返回空字符串</returns>
        public static async Task<string> GetLatestVersion()
        {
            var tagsRequest = new HttpClient();
            tagsRequest.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.29.0");
            tagsRequest.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            List<GithubTag>? tags = await tagsRequest.GetFromJsonAsync<List<GithubTag>>(tagGetUrl);
            if ((tags != null) && (tags.Any()))
            {
                return tags.First().name;
            }
            else
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 启动更新进程
        /// </summary>
        /// <param name="pattern">更新模式</param>
        /// <param name="downloadUrl">下载地址</param>
        /// <returns></returns>
        public static async Task SysUpdate(UpdatePattern pattern, string downloadUrl)
        {
            // 下载文件
            using (Stream updateDownloadStream = new FileStream("Update.zip", FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = await new HttpClient().GetByteArrayAsync(downloadUrl);
                await updateDownloadStream.WriteAsync(buffer);
            }

            // 解压文件
            FileOperator.DeCompressMulti("Update.zip", "Update");
            File.Delete("Update.zip");

            try
            {
                string cmd = string.Empty;
                if(pattern == UpdatePattern.INCREMENT)
                {
                    cmd = "Updater.dll inc";
                }
                else
                {
                    cmd = "Updater.dll full";
                }

                // 调用 Updater 替换文件
                Process.Start(new ProcessStartInfo("dotnet", cmd));

                // 结束当前进程
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                GlobalValues.Logger.Error($"Update failed, {ex.Message}");
            }
        }


        /// <summary>
        /// 检查当前操作系统架构
        /// </summary>
        /// <returns>操作系统架构</returns>
        public static string CheckOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return RuntimeInformation.OSArchitecture switch
                {
                    Architecture.Arm => "linux-arm",
                    Architecture.X64 => "linux-x64",
                    _ => ""
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "osx-x64";
            }
            else
            {
                return RuntimeInformation.OSArchitecture switch
                {
                    Architecture.Arm => "win-arm",
                    Architecture.Arm64 => "win-arm64",
                    Architecture.X64 => "win-x64",
                    Architecture.X86 => "win-x86",
                    _ => ""
                };
            }
        }
    }


    public class GithubTag
    {
        public string name { get; set; }
        public string zipball_url { get; set; }
        public string tarball_url { get; set; }
        public Commit commit { get; set; }
        public string node_id { get; set; }
    }


    public class Commit
    {
        public string sha { get; set; }
        public string url { get; set; }
    }
}
