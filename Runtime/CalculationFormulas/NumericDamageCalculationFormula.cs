using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
    [CreateAssetMenu(menuName = BaseMenuName + nameof(NumericDamageCalculationFormula))]
    public class NumericDamageCalculationFormula : CalculationFormulaDataBase
    {
        [SerializeField] private ParameterCrateData _overallBoostCrate;

        /*
        private void OnValidate()
        {
            Debug.Log(_funcs);
            Debug.Log(_funcs.F);
        }

        private void Reset()
        {
            _funcs = new();
            var second = Expression.Parameter(typeof(float));
            var first = Expression.Parameter(typeof(float));
            var binaryExpression = Expression.Multiply(first, second);

            _funcs.F = Expression.Lambda<Func<float, float, float>>(binaryExpression, first, second);
        }
        */

        protected override List<ulong> BuildDependencies()
        {
            return new List<ulong>
            {
                _overallBoostCrate.Id
            };
        }

        public override float Execute(ParameterDocker docker)
        {
            return 0f;
            /*if (docker.TryGetNumericalDamage(out var crate) == false)
                return 0f;

            if (docker.TryGetOverallDamageBoostCrate(out var boostCrate) == false)
                return crate.GetValue();

            return crate.GetValue() * boostCrate.GetValue();*/
        }
    }
}