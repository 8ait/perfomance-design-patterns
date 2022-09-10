namespace Pooling
{
    using System.Buffers;

    using BenchmarkDotNet.Attributes;

    [MemoryDiagnoser]
    public class ArrayPooling
    {
        private int _itemsCount = 100;
        
        private class Data
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private void Process(Data[] data)
        {
            for (var i = 0; i < _itemsCount; i++)
            {
                data[i].Age++;
            }
        }
        
        [Benchmark]
        public void UsingArrayWithoutPooling()
        {
            var dataArray = new Data[_itemsCount];
            for (var i = 0; i < _itemsCount; i++)
            {
                dataArray[i] ??= new Data();
                dataArray[i].Name = "";
                dataArray[i].Age = i;
            }
            Process(dataArray);
        }

        [Benchmark]
        public void UsingArrayWithPooling()
        {
            var dataArray = ArrayPool<Data>.Shared.Rent(_itemsCount);
            for (var i = 0; i < _itemsCount; i++)
            {
                dataArray[i] ??= new Data();
                dataArray[i].Name = "";
                dataArray[i].Age = i;
            }

            try
            {
                Process(dataArray);
            }
            finally
            {
                ArrayPool<Data>.Shared.Return(dataArray);   
            }
        }
    }
}