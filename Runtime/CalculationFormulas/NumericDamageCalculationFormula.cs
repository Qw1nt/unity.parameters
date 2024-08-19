using System.Collections.Generic;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
    [CreateAssetMenu(menuName = BaseMenuName + nameof(NumericDamageCalculationFormula))]
    public class NumericDamageCalculationFormula : CalculationFormulaDataBase
    {
        [SerializeField] private ParameterCrateData _overallBoostCrate;
        
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