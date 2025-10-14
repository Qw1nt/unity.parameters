using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Extensions;

namespace Parameters.Runtime.CalculationFormulas
{
    public struct FormulaExecutor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Calculate(float input, int parameterId, ComplexParameterContainer container, FormulaElementDescription[] elements)
        {
            var lenght = elements.Length;
            
            for (int i = 0; i < lenght; i++)
            {
                ref var element = ref elements[i];
                element.Calculate(ref elements, parameterId, input, container, true, 1f);
            }

            return elements[^1].CalculatedValue;
        }
    }
}