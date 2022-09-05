namespace Pooling
{
    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            _ = BenchmarkRunner.Run<ArrayPooling>();
        }
    }
}
