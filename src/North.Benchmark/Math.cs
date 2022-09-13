using BenchmarkDotNet.Attributes;

namespace North.Benchmark
{
    [MemoryDiagnoser]
    public class Math
    {
        [Benchmark]
        public void Add()
        {
            Console.WriteLine("Math-Add");
        }
    }
}
