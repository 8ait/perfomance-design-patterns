namespace FrugalObject
{
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>();
        }
    }
}