using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Extensions
{
    public static class FormulaElementDescriptionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Calculate(this ref FormulaElementDescription element,
            ref FormulaElementDescription[] descriptions, ParameterDocker docker)
        {
            var left = default(float);
            var right = default(float);

            switch (element.LeftSource)
            {
                case FormulaDataSource.Parameter:
                    var leftParameter = docker.GetParameter(element.LeftParameterId);
                    left = leftParameter.Value.ParentModifiedValue *
                           leftParameter.Overall.ParentModifiedValue;
                    break;

                case FormulaDataSource.OtherDescriptionValue:
                    left = descriptions[element.LeftIndex].CalculatedValue;
                    break;

                case FormulaDataSource.SimpleValue:
                    left = element.SimpleLeft;
                    break;
            }

            switch (element.RightSource)
            {
                case FormulaDataSource.Parameter:
                    var leftParameter = docker.GetParameter(element.RightParameterId);
                    right = leftParameter.Value.ParentModifiedValue *
                            leftParameter.Overall.ParentModifiedValue;
                    break;

                case FormulaDataSource.OtherDescriptionValue:
                    right = descriptions[element.RightIndex].CalculatedValue;
                    break;

                case FormulaDataSource.SimpleValue:
                    right = element.SimpleRight;
                    break;
            }

            switch (element.OperationType)
            {
                case FormulaOperation.Subtract:
                    element.CalculatedValue = left - right;
                    break;

                case FormulaOperation.Add:
                    element.CalculatedValue = left + right;
                    break;

                case FormulaOperation.Divide:
                    element.CalculatedValue = left / right;
                    break;

                case FormulaOperation.Multiply:
                    element.CalculatedValue = left * right;
                    break;
            }
        }
    }
}