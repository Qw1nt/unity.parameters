using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    public class CalculationFormulaElement
    {
        [SerializeField] private ParameterCrateData _parameter;
        [SerializeField] private string _shortName;

        public ParameterCrateData ParameterCrateData
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
}