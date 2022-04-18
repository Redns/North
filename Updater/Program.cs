using System.Runtime.InteropServices;

namespace Updater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("参数错误 !");
            }
            else
            {
                try
                {
                    if (args[0] == "inc")
                    {
                        Console.WriteLine("[Increment Update] Start...");
                        UpdateInc($"Update/{CheckOSPlatform()}");
                        Console.WriteLine("[Increment Update] Finished");
                    }
                    else
                    {
                        Console.WriteLine("[Full Update] Start...");
                        UpdateInc($"Update/{CheckOSPlatform()}");
                        Console.WriteLine("[Full Update] Finished");
                    }
                }
                catch (Exception ex)
                {
                    File.WriteAllText("updater.log", ex.Message);
                    Console.WriteLine($"Update failed, {ex.Message}");
                }
            }

            Directory.Delete("Update");
        }


        /// <summary>
        /// 增量更新
        /// </summary>
        public static void UpdateInc(string dir)
        {
            DirectoryInfo dirInfo = new(dir);

            var releaseDirs = dirInfo.GetDirectories();
            var releasefiles = dirInfo.GetFiles();

            // 复制文件夹
            foreach(var releaseDir in releaseDirs)
            {
                if(releaseDir.Name != "Data")
                {
                    if (Directory.Exists(releaseDir.Name))
                    {
                        Directory.Delete(releaseDir.Name, true);
                    }
                    new DirectoryInfo($"{dir}/{releaseDir.Name}").MoveTo(releaseDir.Name);
                }
            }

            // 复制文件
            foreach(var releaseFile in releasefiles)
            {
                if (File.Exists(releaseFile.Name))
                {
                    File.Delete(releaseFile.Name);
                }
                new FileInfo($"{dir}/{releaseFile.Name}").MoveTo(releaseFile.Name);
            }
        }


        /// <summary>
        /// 全更新
        /// </summary>
        public static void UpdateFull(string dir)
        {
            DirectoryInfo dirInfo = new(dir);

            var releaseDirs = dirInfo.GetDirectories();
            var releasefiles = dirInfo.GetFiles();

            // 复制文件夹
            foreach (var releaseDir in releaseDirs)
            {
                if (Directory.Exists(releaseDir.Name))
                {
                    Directory.Delete(releaseDir.Name, true);
                }
                new DirectoryInfo($"{dir}/{releaseDir.Name}").MoveTo(releaseDir.Name);
            }

            // 复制文件
            foreach (var releaseFile in releasefiles)
            {
                if (File.Exists(releaseFile.Name))
                {
                    File.Delete(releaseFile.Name);
                }
                new FileInfo($"{dir}/{releaseFile.Name}").MoveTo(releaseFile.Name);
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
}