using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Высокопроизводительный набор int значений (HashSet<int>) с открытым адресованием и удалением со сдвигом кластеров.
/// </summary>
namespace Parameters.Runtime.Collections
{
    public sealed class FastIntHashSet
    {
        private const byte Empty = 0;
        private const byte Occupied = 1;

        private int[] _keys;
        private byte[] _states;
        private int _count;
        private int _threshold;
        private int _mask;

        public int Count => _count;

        public FastIntHashSet(int capacity = 16)
        {
            int size = 1;
            while (size < capacity) size <<= 1;
            _keys = new int[size];
            _states = new byte[size];
            _mask = size - 1;
            _threshold = size >> 1;
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(int key)
        {
            int mask = _mask;
            int idx = key.GetHashCode() & mask;
            // поиск пустой ячейки или существующего ключа
            while (_states[idx] == Occupied)
            {
                if (_keys[idx] == key) return false;
                idx = (idx + 1) & mask;
            }

            _keys[idx] = key;
            _states[idx] = Occupied;
            if (_count++ >= _threshold) Resize();
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int key)
        {
            int mask = _mask;
            int idx = key.GetHashCode() & mask;
            while (_states[idx] != Empty)
            {
                if (_states[idx] == Occupied && _keys[idx] == key) return true;
                idx = (idx + 1) & mask;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(int key)
        {
            int mask = _mask;
            int idx = key.GetHashCode() & mask;
            while (_states[idx] != Empty)
            {
                if (_states[idx] == Occupied && _keys[idx] == key)
                {
                    ShiftRemove(idx);
                    _count--;
                    return true;
                }

                idx = (idx + 1) & mask;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ShiftRemove(int deleteIdx)
        {
            var keys = _keys;
            var states = _states;
            int mask = _mask;
            int i = deleteIdx;
            while (true)
            {
                i = (i + 1) & mask;
                if (states[i] == Empty) break;
                int home = keys[i].GetHashCode() & mask;
                int distI = (i - home) & mask;
                int distDel = (deleteIdx - home) & mask;
                if (distI >= distDel)
                {
                    keys[deleteIdx] = keys[i];
                    deleteIdx = i;
                }
            }

            states[deleteIdx] = Empty;
        }

        public void Clear()
        {
            Array.Clear(_states, 0, _states.Length);
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Resize()
        {
            int newSize = _keys.Length << 1;
            int[] oldKeys = _keys;
            byte[] oldStates = _states;

            _keys = new int[newSize];
            _states = new byte[newSize];
            _mask = newSize - 1;
            _threshold = newSize >> 1;
            _count = 0;

            for (int i = 0; i < oldKeys.Length; i++)
            {
                if (oldStates[i] == Occupied)
                    Add(oldKeys[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator
        {
            private readonly int[] _keys;
            private readonly byte[] _states;
            private readonly int _length;
            private int _index;
            private int _remaining;
            private int _current;

            internal Enumerator(FastIntHashSet set)
            {
                _keys = set._keys;
                _states = set._states;
                _length = _states.Length;
                _index = 0;
                _remaining = set._count;
                _current = default;
            }

            public int Current => _current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_remaining <= 0) return false;
                int idx = _index;
                while (idx < _length)
                {
                    if (_states[idx] == Occupied)
                    {
                        _current = _keys[idx];
                        _index = idx + 1;
                        _remaining--;
                        return true;
                    }

                    idx++;
                }

                _remaining = 0;
                return false;
            }

            public void Reset() => throw new NotSupportedException();

            public void Dispose()
            {
            }
        }
    }
}
