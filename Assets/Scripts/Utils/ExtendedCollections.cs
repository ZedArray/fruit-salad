using System;
using System.Collections;
using System.Collections.Generic;

#region MinHeap

public class MinHeap<T>
{
    private readonly List<T> heap = new();
    private readonly IComparer<T> comparer;

    public MinHeap(IComparer<T>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public int Count => heap.Count;

    public void Clear() => heap.Clear();

    public T Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException();

        return heap[0];
    }

    public void Push(T item)
    {
        heap.Add(item);

        int i = heap.Count - 1;

        while (i > 0)
        {
            int parent = (i - 1) / 2;

            if (comparer.Compare(heap[parent], item) <= 0)
                break;

            heap[i] = heap[parent];
            i = parent;
        }

        heap[i] = item;
    }

    public T Pop()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException();

        T root = heap[0];
        T last = heap[^1];

        heap.RemoveAt(heap.Count - 1);

        if (heap.Count == 0)
            return root;

        int i = 0;

        while (true)
        {
            int left = i * 2 + 1;
            int right = left + 1;

            if (left >= heap.Count)
                break;

            int smallest = left;

            if (right < heap.Count &&
                comparer.Compare(heap[right], heap[left]) < 0)
            {
                smallest = right;
            }

            if (comparer.Compare(last, heap[smallest]) <= 0)
                break;

            heap[i] = heap[smallest];
            i = smallest;
        }

        heap[i] = last;

        return root;
    }
}

#endregion

#region Deque

public class Deque<T> : IEnumerable<T>
{
    private T[] buffer;
    private int head;
    private int tail;

    public int Count { get; private set; }

    public Deque(int capacity = 8)
    {
        buffer = new T[Math.Max(4, capacity)];
    }

    public void AddFront(T item)
    {
        EnsureCapacity();

        head = (head - 1 + buffer.Length) % buffer.Length;
        buffer[head] = item;
        Count++;
    }

    public void AddBack(T item)
    {
        EnsureCapacity();

        buffer[tail] = item;
        tail = (tail + 1) % buffer.Length;
        Count++;
    }

    public T RemoveFront()
    {
        if (Count == 0)
            throw new InvalidOperationException();

        T value = buffer[head];
        head = (head + 1) % buffer.Length;
        Count--;

        return value;
    }

    public T RemoveBack()
    {
        if (Count == 0)
            throw new InvalidOperationException();

        tail = (tail - 1 + buffer.Length) % buffer.Length;
        T value = buffer[tail];
        Count--;

        return value;
    }

    public T PeekFront()
    {
        if (Count == 0)
            throw new InvalidOperationException();

        return buffer[head];
    }

    public T PeekBack()
    {
        if (Count == 0)
            throw new InvalidOperationException();

        return buffer[(tail - 1 + buffer.Length) % buffer.Length];
    }

    private void EnsureCapacity()
    {
        if (Count < buffer.Length)
            return;

        int newSize = buffer.Length * 2;
        T[] newBuffer = new T[newSize];

        for (int i = 0; i < Count; i++)
            newBuffer[i] = buffer[(head + i) % buffer.Length];

        buffer = newBuffer;
        head = 0;
        tail = Count;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return buffer[(head + i) % buffer.Length];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

#endregion

#region OrderedSet

public class OrderedSet<T> : IEnumerable<T>
{
    private readonly LinkedList<T> list = new();
    private readonly Dictionary<T, LinkedListNode<T>> map = new();

    public int Count => map.Count;

    public bool Add(T item)
    {
        if (map.ContainsKey(item))
            return false;

        var node = list.AddLast(item);
        map[item] = node;

        return true;
    }

    public bool Remove(T item)
    {
        if (!map.TryGetValue(item, out var node))
            return false;

        list.Remove(node);
        map.Remove(item);

        return true;
    }

    public bool Contains(T item) => map.ContainsKey(item);

    public void Clear()
    {
        map.Clear();
        list.Clear();
    }

    public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

#endregion

#region MultiMap

public class MultiMap<TKey, TValue>
{
    private readonly Dictionary<TKey, List<TValue>> map = new();

    public void Add(TKey key, TValue value)
    {
        if (!map.TryGetValue(key, out var list))
        {
            list = new List<TValue>();
            map[key] = list;
        }

        list.Add(value);
    }

    public bool Remove(TKey key, TValue value)
    {
        if (!map.TryGetValue(key, out var list))
            return false;

        bool removed = list.Remove(value);

        if (list.Count == 0)
            map.Remove(key);

        return removed;
    }

    public IReadOnlyList<TValue> GetValues(TKey key)
    {
        return map.TryGetValue(key, out var list)
            ? list
            : Array.Empty<TValue>();
    }

    public bool ContainsKey(TKey key) => map.ContainsKey(key);
}

#endregion

#region BiDictionary

public class BiDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, TValue> forward = new();
    private readonly Dictionary<TValue, TKey> reverse = new();

    public int Count => forward.Count;

    public void Add(TKey key, TValue value)
    {
        forward.Add(key, value);
        reverse.Add(value, key);
    }

    public bool RemoveByKey(TKey key)
    {
        if (!forward.TryGetValue(key, out var value))
            return false;

        forward.Remove(key);
        reverse.Remove(value);

        return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
        => forward.TryGetValue(key, out value);

    public bool TryGetKey(TValue value, out TKey key)
        => reverse.TryGetValue(value, out key);
}

#endregion

#region DisjointSet

public class DisjointSet<T>
{
    private readonly Dictionary<T, T> parent = new();
    private readonly Dictionary<T, int> rank = new();

    public void MakeSet(T item)
    {
        if (parent.ContainsKey(item))
            return;

        parent[item] = item;
        rank[item] = 0;
    }

    public T Find(T item)
    {
        if (!parent.ContainsKey(item))
            MakeSet(item);

        if (!EqualityComparer<T>.Default.Equals(parent[item], item))
            parent[item] = Find(parent[item]);

        return parent[item];
    }

    public void Union(T a, T b)
    {
        a = Find(a);
        b = Find(b);

        if (EqualityComparer<T>.Default.Equals(a, b))
            return;

        int rankA = rank[a];
        int rankB = rank[b];

        if (rankA < rankB)
        {
            parent[a] = b;
        }
        else if (rankA > rankB)
        {
            parent[b] = a;
        }
        else
        {
            parent[b] = a;
            rank[a]++;
        }
    }
}

#endregion

#region RingBuffer

public class RingBuffer<T> : IEnumerable<T>
{
    private readonly T[] buffer;
    private int start;

    public int Count { get; private set; }
    public int Capacity => buffer.Length;

    public RingBuffer(int capacity)
    {
        buffer = new T[capacity];
    }

    public void Add(T item)
    {
        if (Count < Capacity)
        {
            buffer[(start + Count) % Capacity] = item;
            Count++;
        }
        else
        {
            buffer[start] = item;
            start = (start + 1) % Capacity;
        }
    }

    public T this[int index]
    {
        get
        {
            if ((uint)index >= Count)
                throw new ArgumentOutOfRangeException();

            return buffer[(start + index) % Capacity];
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

#endregion