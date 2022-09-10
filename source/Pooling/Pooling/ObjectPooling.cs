namespace Pooling
{
    using System;

    using BenchmarkDotNet.Attributes;

    using Microsoft.Extensions.ObjectPool;

    [MemoryDiagnoser]
    public class ObjectPooling
    {
        private readonly TestObject _test;

        private readonly DefaultObjectPool<TestObject> _defaultObjectPool =
            new(new DefaultPooledObjectPolicy<TestObject>());

        private class TestObject
        {
            public string Name { get; set; }

            public TestObject()
            {
                const int l = 5_000;
                Span<char> testString = stackalloc char[l];
                for (var i = 0; i < l; i++)
                {
                    testString[i] = 'a';
                }

                Name = testString.ToString();
            }
        }

        private void Proccess(TestObject testObject)
        {
        }

        [Benchmark]
        public void ObjectWithPooling()
        {
            var test = _defaultObjectPool.Get();
            Proccess(test);
            _defaultObjectPool.Return(test);
        }

        [Benchmark]
        public void ObjectWithoutPooling()
        {
            var test = new TestObject();
            Proccess(test);
        }
    }
}