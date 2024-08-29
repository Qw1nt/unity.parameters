using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public struct ParameterRawValue : IEqualityComparer<ParameterRawValue>
    {
        public readonly uint Hash;
        
        public float CleanValue;
        public float ParentModifiedValue;

        public ParameterRawValue(float cleanValue = 0f)
        {
            var time = DateTime.Now;
            Hash = (uint)unchecked(time.Ticks * time.Millisecond);
            
            CleanValue = cleanValue;
            ParentModifiedValue = 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ParameterRawValue x, ParameterRawValue y)
        {
            return x.Hash == y.Hash;
        }

        public int GetHashCode(ParameterRawValue obj)
        {
            return (int)Hash;
        }
    }
}