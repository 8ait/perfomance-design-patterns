namespace FrugalObject
{
    using System.Collections.Generic;

    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class Benchmark
    {
        [Benchmark]
        public void UseCompactListWithOneObject()
        {
            var compactList = new CompactList<int> { 1 };
            foreach (var item in compactList)
            {
                var i = item * item;
            }
        }

        [Benchmark]
        public void UseListWithOneObject()
        {
            var list = new List<int> { 1 };
            foreach (var item in list)
            {
                var i = item * item;
            }
        }
        
        [Benchmark]
        public void UseCompactListWithOneMoreObject()
        {
            var compactList = new CompactList<int> { 1, 2, 3, 4 };
            foreach (var item in compactList)
            {
                var i = item * item;
            }
        }

        [Benchmark]
        public void UseListWithOneMoreObject()
        {
            var list = new List<int> { 1, 2, 3, 4 };
            foreach (var item in list)
            {
                var i = item * item;
            }
        }
    }
}