// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace go.Components
{
    public class PartialArray<T> : IReadOnlyList<T>
    {
        private T[] _array;
        public int Count { get; private set; }

        public T this[int index] => _array[index];

        public void Clear()
        {
            Count = 0;
        }

        public void Add(T value)
        {
            if (_array == null)
            {
                _array = new T[16];
            }
            else if (_array.Length == Count)
            {
                T[] newArray = new T[_array.Length + _array.Length / 2];
                Array.Copy(_array, 0, newArray, 0, _array.Length);
                _array = newArray;
            }

            _array[Count++] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new PartialArrayEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PartialArrayEnumerator<T>(this);
        }
    }

    public struct PartialArrayEnumerator<T> : IEnumerator<T>
    {
        private PartialArray<T> _partialArray;
        private int _currentIndex;

        public PartialArrayEnumerator(PartialArray<T> partialArray)
        {
            _partialArray = partialArray;
            _currentIndex = -1;
        }

        public T Current => _partialArray[_currentIndex];
        object IEnumerator.Current => _partialArray[_currentIndex];

        public void Dispose()
        { }

        public bool MoveNext()
        {
            _currentIndex++;
            return _currentIndex < _partialArray.Count;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }
    }
}
