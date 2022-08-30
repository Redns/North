using System.Diagnostics;

namespace North.Launcher
{
    class Program
    {
        public static async Task Main()
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