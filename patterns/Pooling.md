# Pooling
Использование пулов готовых ресурсов не новая идея, например тот же самый ThreadPool. Иногда возникают задачи, когда ресурсы можно брать из пула и переиспользовать.

- Проблема: нетривиальная цена выделения памяти при создании объектов. Как правило это прямые накладные расходы (аллоцирование памяти / нагрузка на инициализацию), либо непрямые накладные расходы (нагрузка на GC или фрагментация), либо и те и те накладные расходы сразу.
- Решение: вместо того чтобы каждый раз создавать новый объект, можно переиспользовать объекты из заранее выделеного пула. 
- Преимущество: незначительная нагрузка на GC, нет лишнего аллоцирования памяти. 
- Последствия: логика работы с памятью становится нетривиальной, требует от разработчика знать о правильном заимствовании и освобождении ресурсов из пула.

## Array pooling [microsoft docs](https://docs.microsoft.com/ru-ru/dotnet/api/system.buffers.arraypool-1)
- вместо создания массива выделяет массив из пула массивов.
- rent возвращает массив минимальной необходимой длиной из явно определенных длин.
- экземпляр Shared содержит:
  - 17 корзин с массивами длинной: 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, каждая корзина содержит максимум 50 массивов.
- они инициализируются по требованию.
- они не очищаются, когда арендуются или возвращаются в пул, это нужно делать явно.
- trimming mechanism - так как у массива из пула заранее определена длина, разработчику нужно позаботится о проверке неиспольуземой части массива в каких-либо логических операциях.

```c#
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
    Process(dataArray);
    ArrayPool<Data>.Shared.Return(dataArray);
}
```

Получаем следующие результаты:

```
|                   Method |       Mean |    Error |   StdDev |   Gen0 | Allocated |
|------------------------- |-----------:|---------:|---------:|-------:|----------:|
| UsingArrayWithoutPooling | 1,355.4 ns | 19.23 ns | 17.04 ns | 2.5635 |    4024 B |
|    UsingArrayWithPooling |   465.7 ns |  8.95 ns |  9.57 ns |      - |         - |
```

## Memory pooling [microsoft docs](https://docs.microsoft.com/ru-ru/dotnet/api/system.buffers.memorypool-1)
Представляет собой пул из блоков Memory\<T\>.
- MemoryPool\<T\>.Shared - основан на ArrayPool
- SlabMemoryPool : MemoryPool\<byte\> - пул памяти, который использует Kestrel

```c#
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
```

Получаем такие результаты:

```
|                  Method |      Mean |    Error |   StdDev |   Gen0 | Allocated |
|------------------------ |----------:|---------:|---------:|-------:|----------:|
|    UseMemoryWithPooling | 125.86 ns | 0.513 ns | 0.455 ns | 0.0153 |      24 B |
| UseMemoryWithoutPooling |  97.34 ns | 0.889 ns | 0.788 ns | 0.1428 |     224 B |
```

## Object pooling [microsoft docs](https://docs.microsoft.com/ru-ru/aspnet/core/performance/objectpool)
Редко используется, так как накладные расходы на использование часто перекрывают его достоинства, но можно уменьшить нагрузку на память в случаях:
 - на тяжелое и затратное создание объектов;
 - на объект который часто создается и сам по себе много весит.

```c#
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
```

Получаем такие результаты:
```
|               Method |        Mean |     Error |    StdDev |   Gen0 | Allocated |
|--------------------- |------------:|----------:|----------:|-------:|----------:|
|    ObjectWithPooling |    25.90 ns |  0.451 ns |  0.602 ns |      - |         - |
| ObjectWithoutPooling | 5,499.84 ns | 94.234 ns | 88.147 ns | 6.4087 |   10048 B |

```