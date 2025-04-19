using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public sealed unsafe class FastIntHashSet 
{
    private const uint EmptyKey = 0xFFFFFFFFu;

    private uint[] _keys;
    private int _count;
    private int _threshold;
    private int _mask;

    public int Count => _count;

    public FastIntHashSet(int capacity = 16)
    {
        int size = 1;
        while (size < capacity) size <<= 1;
        _mask = size - 1;
        _threshold = size >> 1;
        _keys = new uint[size];
        for (int i = 0; i < size; i++) _keys[i] = EmptyKey;
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Add(int key)
    {
        uint uKey = (uint)key;
        int mask = _mask;
        int idx = (int)(uKey & (uint)mask);
        int dist = 0;

        fixed (uint* keys = _keys)
        {
            while (true)
            {
                uint existing = keys[idx];
                if (existing == EmptyKey)
                {
                    keys[idx] = uKey;
                    if (_count++ >= _threshold) Resize();
                    return true;
                }
                if (existing == uKey) return false;

                int home = (int)(existing & (uint)mask);
                int existingDist = (idx - home) & mask;
                if (existingDist < dist)
                {
                    // Robin Hood swap
                    keys[idx] = uKey;
                    uKey = existing;
                    dist = existingDist;
                }
                idx = (idx + 1) & mask;
                dist++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(int key)
    {
        uint uKey = (uint)key;
        int mask = _mask;
        int idx = (int)(uKey & (uint)mask);
        int dist = 0;

        fixed (uint* keys = _keys)
        {
            while (true)
            {
                uint existing = keys[idx];
                if (existing == EmptyKey) return false;
                if (existing == uKey) return true;
                int home = (int)(existing & (uint)mask);
                int existingDist = (idx - home) & mask;
                if (existingDist < dist) return false; // ранний выход
                idx = (idx + 1) & mask;
                dist++;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int key)
    {
        uint uKey = (uint)key;
        int mask = _mask;
        int idx = (int)(uKey & (uint)mask);

        fixed (uint* keys = _keys)
        {
            // поиск элемента
            while (true)
            {
                uint existing = keys[idx];
                if (existing == EmptyKey) return false;
                if (existing == uKey) break;
                int home = (int)(existing & (uint)mask);
                int existingDist = (idx - home) & mask;
                if (existingDist == 0) return false;
                idx = (idx + 1) & mask;
            }
            // backshift-remove
            int next = (idx + 1) & mask;
            while (true)
            {
                uint existing = keys[next];
                if (existing == EmptyKey) break;
                int home = (int)(existing & (uint)mask);
                int dist = (next - home) & mask;
                if (dist == 0) break;
                keys[idx] = existing;
                idx = next;
                next = (next + 1) & mask;
            }
            keys[idx] = EmptyKey;
            _count--;
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        for (int i = 0; i < _keys.Length; i++)
            _keys[i] = EmptyKey;
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Resize()
    {
        uint[] oldKeys = _keys;
        int newSize = oldKeys.Length << 1;
        _mask = newSize - 1;
        _threshold = newSize >> 1;
        _keys = new uint[newSize];
        for (int i = 0; i < newSize; i++) _keys[i] = EmptyKey;
        _count = 0;
        foreach (uint k in oldKeys)
            if (k != EmptyKey)
                Add((int)k);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new Enumerator(_keys, _count);

    public struct Enumerator
    {
        private readonly uint[] _keys;
        private readonly int _length;
        private int _index;
        private int _remaining;
        private int _current;

        internal Enumerator(uint[] keys, int count)
        {
            _keys = keys;
            _length = keys.Length;
            _index = 0;
            _remaining = count;
            _current = default;
        }

        public int Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_remaining <= 0) return false;
            while (_index < _length)
            {
                uint k = _keys[_index++];
                if (k != EmptyKey)
                {
                    _current = (int)k;
                    _remaining--;
                    return true;
                }
            }
            _remaining = 0;
            return false;
        }

        public void Reset() => throw new NotSupportedException();
        public void Dispose() { }
    }
}