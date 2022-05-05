using ImageBed.Data.Entity;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ImageBed.Common
{
    public class UpdateHelper
    {
        /// <summary>
        /// 获取当前版本号
        /// </summary>
        /// <returns></returns>
        public static string GetLocalVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            if(assembly != null)
            {
                return $"v{assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取最新版本号
        /// </summary>
        /// <param name="checkUrl">数据源</param>
        /// <returns>获取成功返回最新版本号, 否则返回空字符串</returns>
        public static async Task<string> GetLatestVersion(string checkUrl)
        {
            var tagsRequest = new HttpClient();
            tagsRequest.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.29.0");
            tagsRequest.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            List<GithubTag>? tags = await tagsRequest.GetFromJsonAsync<List<GithubTag>>(checkUrl);
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
        /// 版本号格式为 A.B.C
        /// A：指大版本号，程序大量修改/更换架构时改变
        /// B：新增/删除功能
        /// C：Bug修复/功能优化
        /// </summary>
        /// <param name="v1">版本号1</param>
        /// <param name="v2">版本号2</param>
        /// <returns>相同返回0，v1 > v2返回大于0，v1 < v2返回小于0，输入参数错误返回null</returns>
        public static int? VersionCompare(string v1, string v2)
        {
            int virtualDimensionA = 100000, virtualDimensionB = 1000, virtualDimensionC = 1;

            string[] v1Pics = v1.Split(".");
            string[] v2Pics = v2.Split(".");

            if ((v1Pics.Length != 3) || (v2Pics.Length != 3))
            {
                return null;
            }
            else
            {
                int v1A = int.Parse(v1Pics[0]);
                int v1B = int.Parse(v1Pics[1]);
                int v1C = int.Parse(v1Pics[2]);

                int v2A = int.Parse(v2Pics[0]);
                int v2B = int.Parse(v2Pics[1]);
                int v2C = int.Parse(v2Pics[2]);

                int v1Total = v1A * virtualDimensionA + v1B * virtualDimensionB + v1C * virtualDimensionC;
                int v2Total = v2A * virtualDimensionA + v2B * virtualDimensionB + v2C * virtualDimensionC;

                return v1Total - v2Total;
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
            string platform = CheckOSPlatform();

            try
            {
                // 下载文件
                using (Stream updateDownloadStream = new FileStream("Update.zip", FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = await new HttpClient().GetByteArrayAsync(downloadUrl);
                    await updateDownloadStream.WriteAsync(buffer);
                }

                // 解压文件, 更新文件路径为 Update/{platform}
                FileOperator.DeCompressMulti("Update.zip", "Update");
                File.Delete("Update.zip");

                // 提取 Updater.dll 和 updater.config
                new FileInfo($"Update/{platform}/Updater.dll").MoveTo("Updater.dll", true);
                new FileInfo($"Update/{platform}/updater.config").MoveTo("updater.config", true);

                // 调用 Updater.dll 更新
                if (pattern == UpdatePattern.INCREMENT)
                {
                    Process.Start(new ProcessStartInfo("dotnet", $"Updater.dll inc Update/{platform}"));
                }
                else
                {
                    Process.Start(new ProcessStartInfo("dotnet", $"Updater.dll full Update/{platform}"));
                }
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
