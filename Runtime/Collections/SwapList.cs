using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Parameters.Runtime.Collections
{
   public class SwapList<T>
    {
        private const int DefaultCapacity = 2;
        private readonly EqualityComparer<T> _comparer;

        public T[] Items;
        public int Length;
        public int Capacity;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SwapList()
        {
            _comparer = EqualityComparer<T>.Default;

            Items = new T[DefaultCapacity];
            Capacity = DefaultCapacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SwapList(int capacity, EqualityComparer<T> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;

            Items = new T[capacity];
            Capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T item)
        {
            if (Length == Capacity)
                Resize(Capacity << 1);

            Items[Length] = item;
            Length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(SwapList<T> other)
        {
            if (other.Length == 0)
                return;

            if (Length + other.Length >= Capacity)
                Resize(Capacity * Mathf.CeilToInt((Length + other.Length) / (float)Capacity) + 1);

            Array.Copy(other.Items, 0, Items, Length, other.Length);
            Length += other.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(T item)
        {
            for (int i = 0; i < Length; i++)
            {
                if (_comparer.Equals(item, Items[i]) == false)
                    continue;

                RemoveAt(i);
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            if (index <= -1 || index >= Length)
                return;

            Items[index] = default;
            (Items[Length - 1], Items[index]) = (Items[index], Items[Length - 1]);
            Length--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator()
        {
            return new Enumerator
            {
                Items = Items,
                Length = Length
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Resize(int newSize)
        {
#if UNITY_EDITOR
            if (newSize < Capacity)
                throw new ArgumentException();
#endif

            Array.Resize(ref Items, newSize);
            Capacity = newSize;
        }

        public ref struct Enumerator
        {
            private T _current;

            public T[] Items;
            public int Length;
            public int Index;

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _current;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (Index >= Length)
                    return false;

                _current = Items[Index];
                Index++;

                return true;
            }
        }
    }
}