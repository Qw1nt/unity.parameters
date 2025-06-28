using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Interfaces;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public class ParameterBuilder : IParameterFactory
    {
        [SerializeField] private ParameterIdProvider _parameterId;

        [SerializeField] private float _flatValue;
        [SerializeField] private float _percentValue = 1f;

        [Space(10f)] [SerializeField] private CalculationFormula _formula;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter CreateParameter(ComplexParameterContainer container)
        {
            var instance = new ComplexParameter((int)_parameterId, _formula.Descriptions, _formula.Dependencies, container);

            instance.AddFlat(_flatValue);
            instance.AddPercent(_percentValue);

            return instance;
        }
    }
}