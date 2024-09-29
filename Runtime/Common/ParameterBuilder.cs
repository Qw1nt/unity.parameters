using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Interfaces;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public class ParameterBuilder : IParameterFactory
    {
#if UNITY_EDITOR
        [Title("$" + nameof(CrateName))]
#endif
        [SerializeField] private ParameterData _parameter;
        [InfoBox("Reserved name for use in formula - value")]

        [Space] [SerializeField] private bool _withDefaultValue;
        [SerializeField, ShowIf(nameof(_withDefaultValue)), Indent] private float _flatValue;
        [SerializeField] private float _percentValue = 1f;

        [PropertySpace(10f)] [SerializeField] private CalculationFormula _formula;

#if UNITY_EDITOR
        private string CrateName => _parameter?.DebugName;
#endif

        public ulong Id => _parameter.Id;

        [ReadOnly] public FormulaElementDescription[] Formula => _formula.Descriptions;

        [ReadOnly] public ulong[] Dependencies => _formula.Dependencies;

#if UNITY_EDITOR
        internal void PrepareFormula()
        {
            _formula.Prepare(_parameter);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter CreateParameter(ComplexParameterContainer container)
        {
            var instance = _parameter.CreateParameter(container, _formula.Descriptions, _formula.Dependencies);

            if (_withDefaultValue == true)
                instance.AddFlat(_flatValue);

            instance.AddPercent(_percentValue);

            return instance;
        }
    }
}