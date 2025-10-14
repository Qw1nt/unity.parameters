using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Extensions
{
    public static class FormulaElementDescriptionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Calculate(this ref FormulaElementDescription element,
            ref FormulaElementDescription[] descriptions,
            int targetId,
            ComplexParameterContainer container,
            bool safeGetParameter = false)
        {
            var left = default(float);
            var right = default(float);

            switch (element.LeftSource)
            {
                case FormulaDataSource.Parameter:
                    var leftParameter = container.Get(element.LeftParameterId);

                    left = element.LeftParameterId == targetId
                        ? leftParameter.CalculatedFlat.ParentModifiedValue
                        : leftParameter.CalculatedFlat.ParentModifiedValue *
                          leftParameter.CalculatedPercent.ParentModifiedValue;
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
                    var rightParameter = container.Get(element.RightParameterId);

                    right = element.RightParameterId == targetId
                        ? rightParameter.CalculatedFlat.ParentModifiedValue
                        : rightParameter.CalculatedFlat.ParentModifiedValue *
                          rightParameter.CalculatedPercent.ParentModifiedValue;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Calculate(this ref FormulaElementDescription element,
            ref FormulaElementDescription[] descriptions,
            int targetId,
            float targetValue,
            ComplexParameterContainer container,
            bool canBeMissingParameters = false,
            float missingParameterValue = 0f)
        {
            var left = default(float);
            var right = default(float);

            switch (element.LeftSource)
            {
                case FormulaDataSource.Parameter:
                    var leftParameter = canBeMissingParameters == false
                        ? container.Get(element.LeftParameterId)
                        : container.SafeGet(element.LeftParameterId);

                    left = leftParameter == null
                        ? missingParameterValue
                        : element.LeftParameterId == targetId
                            ? targetValue
                            : leftParameter.CalculatedFlat.ParentModifiedValue *
                              leftParameter.CalculatedPercent.ParentModifiedValue;
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
                    var rightParameter = canBeMissingParameters == false
                        ? container.Get(element.RightParameterId)
                        : container.SafeGet(element.RightParameterId);

                    right = rightParameter == null
                        ? missingParameterValue
                        : element.RightParameterId == targetId
                            ? targetValue
                            : rightParameter.CalculatedFlat.ParentModifiedValue *
                              rightParameter.CalculatedPercent.ParentModifiedValue;
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