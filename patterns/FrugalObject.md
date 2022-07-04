# Frugal object (Скромный объект)
- Проблема: нужно эффиктивно хранить набор данных, который может принимать разные формы.
- Решение: вместо создания объекта для каждой формы, следует использовать что-то в виде дискримнированного объединения.
- Преимущества: иногда ведет к уменьшению использования памяти и лучшей орагнизации данных в памяти. Преимущества JIT оптимизации, например bound checks.
- Последствия: иногда приводит к более сложному API в сравнении с обычным подходом. Некоторые накладные расходы на производительность на проверки типов и дополнительные обертки.

Ниже используется пример такого объекта - CompactList, когда в большинстве случаев требуется хранить в списке не более одного объекта. Таким образом в качестве места хранения этой переменной мы можем выбрать свойство объекта.

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

Получаем такие результаты.

```
|                          Method |      Mean |    Error |    StdDev |  Gen 0 | Allocated |
|-------------------------------- |----------:|---------:|----------:|-------:|----------:|
|     UseCompactListWithOneObject |  45.33 ns | 0.928 ns |  0.912 ns | 0.0255 |      40 B |
|            UseListWithOneObject |  61.94 ns | 2.324 ns |  6.402 ns | 0.0459 |      72 B |
| UseCompactListWithOneMoreObject | 249.12 ns | 5.783 ns | 16.686 ns | 0.1070 |     168 B |
|        UseListWithOneMoreObject |  65.31 ns | 0.822 ns |  0.729 ns | 0.0459 |      72 B |
```

В итоге из текущей реализации CompactList видно, что его выгодно использовать, когда в списке находится всего один объект, но при этом использование памяти возрастает почти в 2 раза при большем количестве объектов.