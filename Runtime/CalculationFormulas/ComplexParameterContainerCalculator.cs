using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Extensions;

namespace Parameters.Runtime.CalculationFormulas
{
    public struct ComplexParameterContainerCalculator
    {
        private const float OneHundredPercent = 1f;

        public static void Calculate(ComplexParameterContainer container)
        {
            if (container.CalculationBuffer.Count == 0)
                return;

            foreach (var parameter in container.CalculationBuffer)
            {
                Calculate(container, container.Get(parameter));

                foreach (var child in container.Children)
                {
                    if (child.TryGet(parameter, out var childParameter, true) == false)
                        continue;

                    Calculate(child, childParameter);
                }
            }

            foreach (var parameterId in container.CalculationBuffer)
            {
                var parameter = container.Get(parameterId);
                var length = parameter.Formula?.Length ?? 0;

                if (parameter.Formula == null || length == 0)
                    continue;
                    
                for (int i = 0; i < length; i++)
                {
                    ref var element = ref parameter.Formula[i];
                    element.Calculate(ref parameter.Formula, parameterId, container);
                }

                var formulaResultValue = parameter.Formula[^1].CalculatedValue;

                parameter.CalculatedFlat.CleanValue += formulaResultValue;
                parameter.CalculatedFlat.ParentModifiedValue += formulaResultValue;
            }

            container.CalculationBuffer.Clear();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Calculate(ComplexParameterContainer container, ComplexParameter complexParameter)
        {
            complexParameter.CalculatedFlat.CleanValue = complexParameter.Flat;
            complexParameter.CalculatedFlat.ParentModifiedValue = complexParameter.Flat;

            complexParameter.CalculatedPercent.CleanValue = complexParameter.Percent;
            complexParameter.CalculatedPercent.ParentModifiedValue = complexParameter.Percent;

            if (container.Parent == null)
                return;

            if (container.Parent.TryGet(complexParameter.Id, out var parentParameter, true) == false)
                return;

            complexParameter.CalculatedFlat.ParentModifiedValue += parentParameter.CalculatedFlat.ParentModifiedValue;
            complexParameter.CalculatedPercent.ParentModifiedValue += OneHundredPercent - parentParameter.CalculatedPercent.ParentModifiedValue;
        }
    }
}