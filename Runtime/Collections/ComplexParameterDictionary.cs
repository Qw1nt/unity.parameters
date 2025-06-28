using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;

public class ComplexParameterDictionary
{
    private int[] _keys;
    private ComplexParameter[] _values;
    private byte[] _occupied; // 0 = empty, 1 = occupied
    private int _capacity;
    private int _mask;
    private int _count;

    private const float MaxLoadFactor = 0.4f;
    private const int MaxRelocations = 32;
    
    public ComplexParameterDictionary(int initialCapacity = 16)
    {
        if (initialCapacity < 1) 
            initialCapacity = 16;
        
        _capacity = NextPowerOfTwo(initialCapacity);
        _mask = _capacity - 1;
        _keys = new int[_capacity];
        _values = new ComplexParameter[_capacity];
        _occupied = new byte[_capacity];
        _count = 0;
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
        return (int)(((uint)key * 2654435761u) & (uint)_mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hash2(int key)
    {
        var h = (uint)key * 40503u;
        
        h ^= h << 13;
        h ^= h >> 17;
        
        return (int)(h & (uint)_mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(int key, ComplexParameter value)
    {
        if (ContainsKey(key))
            throw new ArgumentException($"Key {key} already exists.");

        if (_count + 1 > _capacity * MaxLoadFactor)
            Resize(_capacity * 2);

        if (TryPlace(key, value))
        {
            _count++;
            return;
        }

        var curKey = key;
        var curVal = value;
        var pos = Hash1(curKey);
        
        for (int i = 0; i < MaxRelocations; i++)
        {
            var evictedKey = _keys[pos];
            var evictedVal = _values[pos];
            
            _keys[pos] = curKey;
            _values[pos] = curVal;

            curKey = evictedKey;
            curVal = evictedVal;
            pos = (pos == Hash1(curKey)) ? Hash2(curKey) : Hash1(curKey);

            if (_occupied[pos] != 0)
                continue;
            
            _keys[pos] = curKey;
            _values[pos] = curVal;
            _occupied[pos] = 1;
            _count++;
            
            return;
        }

        Resize(_capacity * 2);
        Add(curKey, curVal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryPlace(int key, ComplexParameter value)
    {
        var i1 = Hash1(key);
        
        if (_occupied[i1] == 0)
        {
            _keys[i1] = key;
            _values[i1] = value;
            _occupied[i1] = 1;
            return true;
        }
        
        var i2 = Hash2(key);
        
        if (_occupied[i2] == 0)
        {
            _keys[i2] = key;
            _values[i2] = value;
            _occupied[i2] = 1;
            return true;
        }
        
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsKey(int key)
    {
        var i1 = Hash1(key);
        
        if (_occupied[i1] == 1 && _keys[i1] == key)
            return true;
        
        var i2 = Hash2(key);
        return _occupied[i2] == 1 && _keys[i2] == key;
    }

    public ComplexParameter this[int key]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            int i1 = Hash1(key);
            if (_occupied[i1] == 1 && _keys[i1] == key)
                return _values[i1];
            int i2 = Hash2(key);
            if (_occupied[i2] == 1 && _keys[i2] == key)
                return _values[i2];
            throw new KeyNotFoundException($"Key {key} not found.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (ContainsKey(key))
            {
                var i1 = Hash1(key);
                
                if (_occupied[i1] == 1 && _keys[i1] == key)
                {
                    _values[i1] = value;
                    return;
                }
                
                var i2 = Hash2(key);
                _values[i2] = value;
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
        var i1 = Hash1(key);
        
        if (_occupied[i1] == 1 && _keys[i1] == key)
        {
            _occupied[i1] = 0;
            _count--;
            return true;
        }
        
        var i2 = Hash2(key);

        if (_occupied[i2] != 1 || _keys[i2] != key) 
            return false;
        
        _occupied[i2] = 0;
        _count--;
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Array.Clear(_occupied, 0, _capacity);
        _count = 0;
    }

    private void Resize(int newCapacity)
    {
        var oldKeys = _keys;
        var oldVals = _values;
        var oldOcc = _occupied;
        var oldCap = _capacity;

        _capacity = NextPowerOfTwo(newCapacity);
        _mask = _capacity - 1;
        _keys = new int[_capacity];
        _values = new ComplexParameter[_capacity];
        _occupied = new byte[_capacity];
        _count = 0;

        for (int i = 0; i < oldCap; i++)
        {
            if (oldOcc[i] == 1)
                Add(oldKeys[i], oldVals[i]);
        }
    }
}
