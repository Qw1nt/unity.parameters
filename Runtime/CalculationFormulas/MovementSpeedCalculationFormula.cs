using System.Collections.Generic;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
    [CreateAssetMenu(menuName = BaseMenuName + nameof(MovementSpeedCalculationFormula))]
    public class MovementSpeedCalculationFormula : CalculationFormulaDataBase
    {
        [SerializeField] private ParameterData _movementSpeedBoost;

        protected override List<ulong> BuildDependencies()
        {
            return new List<ulong>()
            {
                _movementSpeedBoost.Id
            };
        }

        public override float Execute(ParameterDocker docker)
        {
            return 0f;
            /*if (docker.TryGetMovementSpeedCrate(out var movementCrate) == false)
                return 0f;

            if (docker.TryGetOverallMovementSpeedBoost(out var boost) == false)
                return movementCrate.GetValue();

            return movementCrate.GetValue() * boost.GetValue();*/
        }
    }
}