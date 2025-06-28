using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Collections;
using Parameters.Runtime.Common;

public class ComplexParameterListDictionary
{
    private int[] keys;
    private FastList<ComplexParameter>[] values;
    private byte[] occupied; // 0 = empty, 1 = occupied
    private int capacity;
    private int mask;
    private int count;

    private const float MAX_LOAD_FACTOR = 0.4f;
    private const int MAX_RELOCATIONS = 32;

    public int Count => count;

    public ComplexParameterListDictionary(int initialCapacity = 16)
    {
        if (initialCapacity < 1) initialCapacity = 16;
        capacity = NextPowerOfTwo(initialCapacity);
        mask = capacity - 1;
        keys = new int[capacity];
        values = new FastList<ComplexParameter>[capacity];
        occupied = new byte[capacity];
        count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int NextPowerOfTwo(int v)
    {
        v--;
        v |= v >> 1;
        v |= v >> 2;
        v |= v >> 4;
        v |= v >> 8;
        v |= v >> 16;
        return v + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hash1(int key)
    {
        return (int)(((uint)key * 2654435761u) & (uint)mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hash2(int key)
    {
        uint h = (uint)key * 40503u;
        h ^= h << 13;
        h ^= h >> 17;
        return (int)(h & (uint)mask);
    }

    public void Add(int key, FastList<ComplexParameter> value)
    {
        if (ContainsKey(key))
            throw new ArgumentException($"Key {key} already exists.");

        if (count + 1 > capacity * MAX_LOAD_FACTOR)
            Resize(capacity * 2);

        if (TryPlace(key, value))
        {
            count++;
            return;
        }

        int curKey = key;
        FastList<ComplexParameter> curVal = value;
        int pos = Hash1(curKey);
        for (int i = 0; i < MAX_RELOCATIONS; i++)
        {
            int evictedKey = keys[pos];
            FastList<ComplexParameter> evictedVal = values[pos];
            keys[pos] = curKey;
            values[pos] = curVal;

            curKey = evictedKey;
            curVal = evictedVal;
            pos = (pos == Hash1(curKey)) ? Hash2(curKey) : Hash1(curKey);

            if (occupied[pos] == 0)
            {
                keys[pos] = curKey;
                values[pos] = curVal;
                occupied[pos] = 1;
                count++;
                return;
            }
        }

        // Too many relocations: resize and retry
        Resize(capacity * 2);
        Add(curKey, curVal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryPlace(int key, FastList<ComplexParameter> value)
    {
        int i1 = Hash1(key);
        if (occupied[i1] == 0)
        {
            keys[i1] = key;
            values[i1] = value;
            occupied[i1] = 1;
            return true;
        }
        int i2 = Hash2(key);
        if (occupied[i2] == 0)
        {
            keys[i2] = key;
            values[i2] = value;
            occupied[i2] = 1;
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(int key)
    {
        int i1 = Hash1(key);
        if (occupied[i1] == 1 && keys[i1] == key)
            return true;
        int i2 = Hash2(key);
        return occupied[i2] == 1 && keys[i2] == key;
    }

    public FastList<ComplexParameter> this[int key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var i1 = Hash1(key);
            
            if (occupied[i1] == 1 && keys[i1] == key)
                return values[i1];
            
            var i2 = Hash2(key);
            
            if (occupied[i2] == 1 && keys[i2] == key)
                return values[i2];
            
            throw new KeyNotFoundException($"Key {key} not found.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (ContainsKey(key))
            {
                int i1 = Hash1(key);
                if (occupied[i1] == 1 && keys[i1] == key)
                {
                    values[i1] = value;
                    return;
                }
                int i2 = Hash2(key);
                values[i2] = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Remove(int key)
    {
        int i1 = Hash1(key);
        if (occupied[i1] == 1 && keys[i1] == key)
        {
            occupied[i1] = 0;
            count--;
            return true;
        }
        int i2 = Hash2(key);
        if (occupied[i2] == 1 && keys[i2] == key)
        {
            occupied[i2] = 0;
            count--;
            return true;
        }
        return false;
    }

    public void Clear()
    {
        Array.Clear(occupied, 0, capacity);
        count = 0;
    }

    private void Resize(int newCapacity)
    {
        var oldKeys = keys;
        var oldVals = values;
        var oldOcc = occupied;
        int oldCap = capacity;

        capacity = NextPowerOfTwo(newCapacity);
        mask = capacity - 1;
        keys = new int[capacity];
        values = new FastList<ComplexParameter>[capacity];
        occupied = new byte[capacity];
        count = 0;

        for (int i = 0; i < oldCap; i++)
        {
            if (oldOcc[i] == 1)
                Add(oldKeys[i], oldVals[i]);
        }
    }
}
