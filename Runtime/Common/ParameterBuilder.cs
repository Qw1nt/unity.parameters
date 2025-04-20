using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using SaintsField;
using SaintsField.Playa;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public class ParameterBuilder : IParameterFactory
    {
#if UNITY_EDITOR
        [Layout("$" + nameof(CrateName), ELayout.Title | ELayout.TitleOut)]
#endif
        [SerializeField]
        private ParameterData1 _parameter;

        [SerializeField] private ParameterIdProvider _parameterId;

        [InfoBox("Reserved name for use in formula - value")] [SerializeField]
        private float _flatValue;

        [SerializeField] private float _percentValue = 1f;

        [Space(10f)] [SerializeField] private CalculationFormula _formula;

#if UNITY_EDITOR
        private string CrateName => _parameter?.DebugName;
#endif

        public int Id => (int)_parameterId;

        public FormulaElementDescription[] Formula
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _formula.Descriptions;
        }

        public int[] Dependencies
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _formula.Dependencies;
        }

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

            instance.AddFlat(_flatValue);
            instance.AddPercent(_percentValue);

            return instance;
        }
    }
}