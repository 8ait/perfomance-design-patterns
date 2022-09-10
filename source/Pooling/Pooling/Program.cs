namespace Pooling
{
    using System.Reflection;

    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            // _ = BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
            _ = BenchmarkRunner.Run<ObjectPooling>();
        }
    }
}
