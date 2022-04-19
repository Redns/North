using System.Diagnostics;

namespace Updater
{
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">传参参数格式为 dotnet Updater.dll {updateMode} {dir}</param>
        /// <returns></returns>
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length == 2)
                {
                    var updateMode = args[0];                               // 更新模式(inc/full)
                    var updateDir = args[1];                                // 更新文件路径
                    var updateConfig = File.ReadAllText("updater.config");  // 更新配置文件

                    Update(updateMode, updateDir, updateConfig);

                    File.WriteAllText("updater.log", $"Update successfully");
                }
                else
                {
                    File.WriteAllText("updater.log", $"Update failed, wrong args");
                }
            }
            catch(Exception ex)
            {
                File.WriteAllText("updater.log", $"Update failed, {ex.Message}");
            }
            finally
            {
                Process.Start(new ProcessStartInfo("dotnet", "ImageBed.dll"));
                Directory.Delete(args[1], true);
                Directory.Delete(new DirectoryInfo(args[1])?.Parent?.Name ?? "", true);
            }
        }


        /// <summary>
        /// 更新程序
        /// </summary>
        /// <param name="updateMode">更新模式</param>
        /// <param name="dir">更新文件路径(如:Update/win-x64)</param>
        /// <param name="updateConfig">更新配置</param>
        static void Update(string updateMode, string dir, string updateConfig)
        {
            if (updateMode == "inc")
            {
                // 增量更新
                // 删除 updaterConfig 中的忽略文件、文件夹
                string[] paths = updateConfig.Split("\n");
                foreach (string path in paths)
                {
                    string fullpath = $"{dir}/{path}";
                    if (File.Exists(fullpath))
                    {
                        File.Delete(fullpath);
                    }
                    else if (Directory.Exists(fullpath))
                    {
                        Directory.Delete(fullpath, true);
                    }
                }
            }

            // 迁移剩余数据
            DirectoryInfo dirInfo = new(dir);
            var releaseDirs = dirInfo.GetDirectories();
            var releasefiles = dirInfo.GetFiles();

            foreach(var releaseDir in releaseDirs)
            {
                if (Directory.Exists(releaseDir.Name))
                {
                    Directory.Delete(releaseDir.Name, true);
                }
                new DirectoryInfo($"{dir}/{releaseDir.Name}").MoveTo(releaseDir.Name);
            }

            foreach(var releaseFile in releasefiles)
            {
                if (File.Exists(releaseFile.Name))
                {
                    File.Delete(releaseFile.Name);
                }
                new FileInfo($"{dir}/{releaseFile.Name}").MoveTo(releaseFile.Name);
            }
        }
    }
}