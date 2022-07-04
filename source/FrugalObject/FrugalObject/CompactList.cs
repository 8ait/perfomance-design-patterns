namespace FrugalObject
{
    using System.Collections;
    using System.Collections.Generic;

    public struct CompactList<T> : IEnumerable<T>
    {        
        private T _mySingleValue;

        private List<T> _myMultipleValues;

        private bool _isSingleValueIsUsed;

        public void Add(T value)
        {
            if (_myMultipleValues is not null)
                _myMultipleValues.Add(value);

            if (_isSingleValueIsUsed is not true)
            {
                _mySingleValue = value;
                _isSingleValueIsUsed = true;
            }
            else
            {
                if (_myMultipleValues is null)
                {
                    _myMultipleValues = new List<T>();
                }
                _myMultipleValues.Add(_mySingleValue);
                _myMultipleValues.Add(value);
                
                _mySingleValue = default;
            }
        }

        public void Remove(T value)
        {
            if (_myMultipleValues is not null)
            {
                _myMultipleValues.Remove(value);
            } else if (_isSingleValueIsUsed && _mySingleValue.Equals(value))
            {
                _isSingleValueIsUsed = false;
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            if (_myMultipleValues is not null)
                return _myMultipleValues.GetEnumerator();
            
            return new CompactEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal struct CompactEnumerator<T>: IEnumerator<T>
        {
            private CompactList<T> _compactList;

            private int _count;

            public bool MoveNext()
            {
                if (_count == 0 && _compactList._isSingleValueIsUsed)
                {
                    _count++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _compactList._mySingleValue = default;
            }

            public CompactEnumerator(CompactList<T> list)
            {
                _count = default;
                _compactList = list;
                Current = _compactList._mySingleValue;
            }

            public T Current { get; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
            }
        }
    }
}