using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
#if UNITY_EDITOR
    [Serializable]
    public class UsedFormulaParameter 
    {
        [SerializeField] private ParameterData1 _parameter;
        [SerializeField] private ParameterIdProvider _parameterIdProvider;
        [SerializeField] private string _shortName;

        public ParameterData1 ParameterData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parameter;
        }

        public int ParameterId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int)_parameterIdProvider;
        }

        public string ShortName
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _shortName;
        }
    }
#endif
}