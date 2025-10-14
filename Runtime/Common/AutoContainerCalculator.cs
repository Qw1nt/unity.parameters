using System;
using Parameters.Runtime.CalculationFormulas;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    public class AutoContainerCalculator : MonoBehaviour
    {
        private void LateUpdate()
        {
            var containers = ComplexParameterContainerStorage.Containers;

            foreach (var container in containers)
            {
                if (container.CalculationBuffer.length == 0)
                    return;
                
                ComplexParameterContainerCalculator.Calculate(container);
            }
        }
    }
}