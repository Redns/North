using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace North.Benchmark
{
    public class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<Math>();
        }
    }
}