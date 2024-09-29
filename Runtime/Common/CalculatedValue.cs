using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public struct CalculatedValue
    {
        public float CleanValue;
        public float ParentModifiedValue;

        public CalculatedValue(float cleanValue)
        {
            CleanValue = cleanValue;
            ParentModifiedValue = cleanValue;
        }

        public int GetHashCode(CalculatedValue obj)
        {
            return obj.CleanValue.GetHashCode() ^ obj.ParentModifiedValue.GetHashCode();
        }
    }
}