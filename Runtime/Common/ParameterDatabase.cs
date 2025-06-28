using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [CreateAssetMenu]
    public class ParameterDatabase : ScriptableObject
    {
        [SerializeField] private ParameterInfo[] _values;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<ParameterInfo> GetValidRecordsWithAlloc()
        {
            var result = new List<ParameterInfo>();

            foreach (var item in _values)
            {
                if (item.Id != 0)
                    result.Add(item);
            }

            return result;
        }
    }
}