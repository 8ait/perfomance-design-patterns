# Pooling
- Проблема: нетривиальная цена выделения памяти при создании объектов. Как правило это либо прямые накладные расходы, как аллоцирование памяти / нагрузка на инициализацию, либо непрямые накладные расходы, как нагрузка на GC или фрагментация, либо и те и те накладные расходы сразу.
- Решение: вместо того чтобы каждый раз создавать новый объект, можно переиспользовать объекты из заранее выделеного пула. 
- Преимущество: незначительная нагрузка на GC, нет лишнего аллоцирования памяти. 
- Последствия: логика работы с памятью становится нетривиальной, заставляет разработчика думать о наличии сборщика мусора, заимствовать и освобождать ресурсы из пула.

## В .net из под коробки доступны следующие использования пуллинга:
- Array pooling

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

```
|                   Method |       Mean |    Error |   StdDev |   Gen0 | Allocated |
|------------------------- |-----------:|---------:|---------:|-------:|----------:|
| UsingArrayWithoutPooling | 1,355.4 ns | 19.23 ns | 17.04 ns | 2.5635 |    4024 B |
|    UsingArrayWithPooling |   465.7 ns |  8.95 ns |  9.57 ns |      - |         - |
```