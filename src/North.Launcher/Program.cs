using System.Diagnostics;

namespace North.Launcher
{
    class Program
    {
        public delegate void OrderTestDelegate(string message);
        public static event OrderTestDelegate OrderTestEvent;
        public static async Task Main()
        {
            DelegateOrderTest();
        }


        public static async Task DelegateOrderTest()
        {
            for(int i = 0; i < 100; i++)
            {
                var prefix = $"[{i}] ";
                OrderTestEvent += (msg) => Console.WriteLine($"{prefix}{msg}");
            }
            OrderTestEvent("Hello, World");
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

        public static async Task Upload(int delay = 1000)
        {
            await Task.Delay(delay);
            Console.WriteLine($"[Delay {delay} ms]");
        }
    }
}