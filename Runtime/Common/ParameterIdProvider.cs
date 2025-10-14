using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public struct ParameterIdProvider
    {
        [SerializeField] private int _id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(ParameterIdProvider provider)
        {
            return provider._id;
        }
    }
}