using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Extensions;

namespace Parameters.Runtime.CalculationFormulas
{
    public struct SimpleDockerCalculator
    {
        private const float OneHundredPercent = 1f;

        public static void Calculate(ParameterDocker docker)
        {
            if (docker.CalculationBuffer.Count == 0)
                return;

            foreach (var parameter in docker.CalculationBuffer)
            {
                Calculate(docker, docker.GetParameter(parameter));

                foreach (var child in docker.Children)
                {
                    if (child.TryGetCrate(parameter, out var childParameter, true) == false)
                        continue;

                    Calculate(child, childParameter);
                }
            }

            foreach (var parameterId in docker.CalculationBuffer)
            {
                var parameter = docker.GetParameter(parameterId);
                var length = parameter.Formula?.Length ?? 0;

                if (parameter.Formula == null || length == 0)
                {
                    parameter.SaveChanges();
                    continue;
                }
                    
                for (int i = 0; i < length; i++)
                {
                    ref var element = ref parameter.Formula[i];
                    element.Calculate(ref parameter.Formula, docker);
                }

                var formulaResultValue = parameter.Formula[^1].CalculatedValue;

                parameter.Value.CleanValue += formulaResultValue;
                parameter.Value.ParentModifiedValue += formulaResultValue;
                
                parameter.SaveChanges();
            }

            docker.CalculationBuffer.Clear();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Calculate(ParameterDocker docker, Parameter parameter)
        {
            parameter.Value.CleanValue = 0f;
            parameter.Overall.CleanValue = 0f;

            for (int i = 0; i < parameter.Values.length; i++)
                parameter.Value.CleanValue += parameter.Values.data[i].CleanValue;

            for (int i = 0; i < parameter.Overalls.length; i++)
                parameter.Overall.CleanValue += parameter.Overalls.data[i].CleanValue;

            parameter.Value.ParentModifiedValue = parameter.Value.CleanValue;
            parameter.Overall.ParentModifiedValue = parameter.Overall.CleanValue;

            if (docker.Parent == null)
                return;

            if (docker.Parent.TryGetCrate(parameter.Id, out var parentParameter, true) == false)
                return;

            parameter.Value.ParentModifiedValue += parentParameter.Value.ParentModifiedValue;
            parameter.Overall.ParentModifiedValue += OneHundredPercent - parentParameter.Overall.ParentModifiedValue;
        }
    }
}