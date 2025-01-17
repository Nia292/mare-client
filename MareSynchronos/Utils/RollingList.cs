﻿using System.Collections;

namespace MareSynchronos.Utils;

public class RollingList<T> : IEnumerable<T>
{
    private readonly LinkedList<T> _list = new();
    private readonly object _addLock = new();

    public RollingList(int maximumCount)
    {
        if (maximumCount <= 0)
            throw new ArgumentException(message: null, nameof(maximumCount));

        MaximumCount = maximumCount;
    }

    public int MaximumCount { get; }
    public int Count => _list.Count;

    public void Add(T value)
    {
        lock (_addLock)
        {
            if (_list.Count == MaximumCount)
            {
                _list.RemoveFirst();
            }
            _list.AddLast(value);
        }
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _list.Skip(index).First();
        }
    }

    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
