using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
#if UNITY_EDITOR
    [Serializable]
    public class CalculationFormulaElement
    {
        [SerializeField] private ParameterData _parameter;
        [SerializeField] private string _shortName;

        public ParameterData ParameterData
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parameter;
        }

        public string ShortName
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _shortName;
        }
    }
#endif
}