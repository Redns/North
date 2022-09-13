using CZGL.SystemInfo;
using System.Diagnostics;

namespace North.Launcher
{
    class Program
    {
        public static readonly double KB = 1024.0;
        public static readonly double MB = 1024 * 1024.0;
        public static readonly double GB = 1024 * 1024 * 1024.0;

        public static async Task Main()
        {
            // 获取磁盘信息
            var disks = DiskInfo.GetDisks();

            // 获取进程 CPU 占用
            var count = 0;
            var process = ProcessInfo.GetCurrentProcess();
            while (true)
            {
                if(count++ % 5 == 0)
                {
                    await File.ReadAllBytesAsync("src.png");
                }
                await Task.Delay(200);
                Console.WriteLine(ProcessInfo.GetCpuUsage(process));
            }
        }


        /// <summary>
        /// 获取进程运行内存占用
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static long GetProcessRAM(Process process)
        {
            return process.WorkingSet64;
        }


        /// <summary>
        /// 多任务并行处理测试
        /// </summary>
        /// <returns></returns>
        public static async Task MultiTaskTest()
        {
            var stopwatch = new Stopwatch();
            var Tasks = new List<Task>() { Upload(), Upload(2000), Upload(3000) };

            stopwatch.Start();
            while (Tasks.Any())
            {
                Tasks.Remove(await Task.WhenAny(Tasks));
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }


        /// <summary>
        /// 图片上传模拟
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static async Task Upload(int delay = 1000)
        {
            await Task.Delay(delay);
            Console.WriteLine($"[Delay {delay} ms]");
        }
    }
}