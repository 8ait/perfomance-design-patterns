namespace Pooling
{
    using System;
    using System.Buffers;

    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class MemoryPooling
    {
        private void Process(Memory<char> memory)
        {
            var span = memory.Span;
            for (var i = 0; i < 100; i++)
            {
                span[i] = (char)i;
            }
        }
        
        [Benchmark]
        public void UseMemoryWithPooling()
        {
            using var memory = MemoryPool<char>.Shared.Rent();
            Process(memory.Memory);       
        }

        [Benchmark]
        public void UseMemoryWithoutPooling()
        {
            var memory = new Memory<char>(new char[100]);
            Process(memory);
        }
    }
}