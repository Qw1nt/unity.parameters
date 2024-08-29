using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public struct ParameterRawValue : IEquatable<ParameterRawValue>, IEqualityComparer<ParameterRawValue>
    {
        private const ulong Offset = 7_777_777_777_777UL;
        private static ulong LastHash = 1_000_000_000_000UL;
        private static object _locker = new();

        public readonly ulong Hash;

        public float CleanValue;
        public float ParentModifiedValue;
        
        public ParameterRawValue(float cleanValue)
        {
            lock (_locker)
            {
                Hash = unchecked(LastHash * Offset);
                LastHash = Hash;
            }

            CleanValue = cleanValue;
            ParentModifiedValue = cleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is ParameterRawValue other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ParameterRawValue other)
        {
            return Hash == other.Hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ParameterRawValue x, ParameterRawValue y)
        {
            return x.Hash == y.Hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetHashCode(ParameterRawValue obj)
        {
            return (int)Hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (int)Hash;
        }
    }
}