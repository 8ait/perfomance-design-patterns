# Frugal object (Скромный объект)
Представим, что в нашей программе, например веб-сервере, повсюду используются объекты типа список. Чаще всего в списке содержится всего один объект, но мы используем именно список, потому что в редких случаях объектов в списке может быть больше. Известно, что создание списка создает в .net определенные издержки, например bound checks(проверки CLR на выход за пределы массива) или выделение памяти под 4 элемента при неявном указании в конструкторе. В данном случае нам может помочь скромный объект.

- Проблема: нужно эффективно хранить набор данных, который можно представить в разных структурных формах (список, обособленный объект, два обособленных объекта).
- Решение: вместо создания объекта для каждой формы, следует использовать что-то вроде объединения структурных форм и их обработку в зависимости от контекста.
- Преимущества: ведет к уменьшению использования памяти и лучшей организации данных в памяти. Использует преимущества JIT оптимизации, например избегает bound checks, если мы ограничлись использованием скромного объекта в виде обособленного объекта, а не списка.
- Последствия: приводит к более сложному API. Создает дополнительные обертки и расходы на производительность для проверки типов.

Ниже используется пример такого объекта - CompactList, когда в большинстве случаев требуется хранить в списке не более одного объекта.

```c#
public struct CompactList<T> : IEnumerable<T>        
{        
    private T _mySingleValue;

    private List<T> _myMultipleValues;

    ...
}
```

Сделаем несколько тестов на производительность:

```c#
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
```

Получаем слудеющие результаты:

```
|                          Method |      Mean |    Error |    StdDev |  Gen 0 | Allocated |
|-------------------------------- |----------:|---------:|----------:|-------:|----------:|
|     UseCompactListWithOneObject |  45.33 ns | 0.928 ns |  0.912 ns | 0.0255 |      40 B |
|            UseListWithOneObject |  61.94 ns | 2.324 ns |  6.402 ns | 0.0459 |      72 B |
| UseCompactListWithOneMoreObject | 249.12 ns | 5.783 ns | 16.686 ns | 0.1070 |     168 B |
|        UseListWithOneMoreObject |  65.31 ns | 0.822 ns |  0.729 ns | 0.0459 |      72 B |
```

В итоге из текущей реализации CompactList видно, что его выгодно использовать, когда в списке находится всего один объект, но при этом использование памяти возрастает почти в 2 раза при большем количестве объектов.