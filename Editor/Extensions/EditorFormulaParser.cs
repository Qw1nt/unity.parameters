using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Parameters.Runtime.Base;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;
using Parameters.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace Parameters.Editor.Extensions
{
    public static class EditorFormulaParser
    {
        public static void Prepare(SerializedProperty formulaProperty, int requiredId)
        {
            var formula = formulaProperty.FindPropertyRelative("_formula").stringValue;
            var elements = formulaProperty.FindPropertyRelative("_elements");
            var usages = formulaProperty.FindPropertyRelative("_usages");
            var descriptions = formulaProperty.FindPropertyRelative("Descriptions");
            var dependencies = formulaProperty.FindPropertyRelative("Dependencies");
            
            if (string.IsNullOrEmpty(formula) == true)
            {
                usages.ClearArray();
                descriptions.ClearArray();
                dependencies.ClearArray();
                
                formulaProperty.serializedObject.ApplyModifiedProperties();
                return;
            }

            if (formula.Contains("value") == true)
            {
            }
            
            usages.ClearArray();
            descriptions.ClearArray();
            dependencies.ClearArray();

            for (int i = 0; i < elements.arraySize; i++)
            {
                var element = elements.GetArrayElementAtIndex(i);
                var idValue = element.GetIdProperty();
                
                dependencies.InsertArrayElementAtIndex(0);
                var dependency = dependencies.GetArrayElementAtIndex(0);
                dependency.intValue = idValue.intValue;
            }

            ParameterBuilderUsagesFactory.instance.Build(requiredId, elements, usages);

            var elementsMap = new Dictionary<string, int>();
            
            for (int i = 0; i < usages.arraySize; i++)
            {
                var element = usages.GetArrayElementAtIndex(i);
                var id = element.GetIdProperty();
                var shortName = element.FindPropertyRelative("_shortName").stringValue;

                elementsMap.Add(shortName, id.intValue);
            }
            
            var result = new List<HashedFormulaElement>();

            var pointer = 0;
            
            if (char.IsWhiteSpace(formula[^1]) == false)
                formula += ' ';

            while (pointer < formula.Length)
            {
                var hash = formula.GetRawHash(pointer, out var newPointer);

                if (newPointer == -1)
                    break;

                var item = new HashedFormulaElement
                {
                    Expression = formula.Substring(pointer, newPointer - pointer).Replace(" ", ""),
                    Hash = hash,
                    Position = newPointer,
                };

                item.AdjustElementHash();
                result.Add(item);

                if (float.TryParse(item.Expression, NumberStyles.Float, CultureInfo.InvariantCulture, out var simpleValue) == true)
                    item.SimpleValue = simpleValue;
                else if (elementsMap.TryGetValue(item.Expression, out var parameterId) == true)
                    item.ParameterId = parameterId;
                else if(item.Expression.Length > 1 || item.Expression[0].IsReserved() == false)
                    Debug.LogError($"Element \"{item.Expression}\" is not reserved element");

                if (result.Count > 1)
                {
                    for (int i = result.Count - 2; i >= 0; i--)
                    {
                        if (result[i].IsOpenGroup() == true || result[i].IsCloseGroup() == true)
                            continue;

                        item.Left = result[i].Hash;
                        break;
                    }
                }

                pointer = newPointer;
            }
            
            var operators = result
                .Where(x => x.IsOperator() == true)
                .ToList();

            foreach (var element in operators)
                element.MathOperationType = element.GetOperationType();

            SetCorrectRightReferences(operators, result);

            foreach (var item in operators)
            {
                var index = result.IndexOf(item);
                var weight = 0u;

                for (int i = 0; i < index; i++)
                {
                    var element = result[i];

                    if (element.IsOpenGroup() == true)
                        weight += element.GetSymbolWeight();

                    if (element.IsCloseGroup() == true)
                        weight -= element.GetSymbolWeight();
                }

                item.Weight = weight + item.GetSymbolWeight();
            }
            
            var hashMap = result.ToDictionary(x => x.Hash);

              var sortedElements = result.Where(x => x.IsOperator() == true)
                .OrderByDescending(x => x.Weight).ToArray();

            var rawDescriptions = new List<FormulaElementDescription>();

            foreach (var hashedOperator in sortedElements)
            {
                var description = new FormulaElementDescription
                {
                    Hash = hashedOperator.Hash,
                    OperationType = hashedOperator.MathOperationType
                };

                var leftHash = hashedOperator.Left;
                var rightHash = hashedOperator.Right;

                if (leftHash != 0 && hashMap.TryGetValue(leftHash, out var left) == true)
                {
                    description.LeftIndex = TryGetReferenceIndex(rawDescriptions, operators, hashedOperator, left);

                    if (description.LeftIndex == -1)
                    {
                        description.SimpleLeft = left.SimpleValue;
                        description.LeftParameterId = left.ParameterId;
                    }

                    if (description.LeftIndex != -1)
                        description.LeftSource = FormulaDataSource.OtherDescriptionValue;
                    else if (description.LeftParameterId != 0)
                        description.LeftSource = FormulaDataSource.Parameter;
                    else
                        description.LeftSource = FormulaDataSource.SimpleValue;
                }

                if (hashedOperator.Right != 0 && hashMap.TryGetValue(rightHash, out var right) == true)
                {
                    description.RightIndex = TryGetReferenceIndex(rawDescriptions, operators, hashedOperator, right);

                    if (description.RightIndex == -1)
                    {
                        description.SimpleRight = right.SimpleValue;
                        description.RightParameterId = right.ParameterId;
                    }

                    if (description.RightIndex != -1)
                        description.RightSource = FormulaDataSource.OtherDescriptionValue;
                    else if (description.RightParameterId != 0)
                        description.RightSource = FormulaDataSource.Parameter;
                    else
                        description.RightSource = FormulaDataSource.SimpleValue;
                }

                rawDescriptions.Add(description);
            }

            for (int i = rawDescriptions.Count - 1; i >= 0; i--)
            {
                var description = rawDescriptions[i];
                
                descriptions.InsertArrayElementAtIndex(0);    
                var element = descriptions.GetArrayElementAtIndex(0);

                element.FindPropertyRelative(nameof(FormulaElementDescription.Hash)).ulongValue = description.Hash;
                
                element.FindPropertyRelative(nameof(FormulaElementDescription.LeftSource)).intValue = (int)description.LeftSource;
                element.FindPropertyRelative(nameof(FormulaElementDescription.RightSource)).intValue = (int)description.RightSource;
                
                element.FindPropertyRelative(nameof(FormulaElementDescription.LeftIndex)).intValue = description.LeftIndex;
                element.FindPropertyRelative(nameof(FormulaElementDescription.RightIndex)).intValue = description.RightIndex;
                
                element.FindPropertyRelative(nameof(FormulaElementDescription.LeftParameterId)).intValue = description.LeftParameterId;
                element.FindPropertyRelative(nameof(FormulaElementDescription.RightParameterId)).intValue = description.RightParameterId;   
                
                element.FindPropertyRelative(nameof(FormulaElementDescription.SimpleLeft)).floatValue = description.SimpleLeft;
                element.FindPropertyRelative(nameof(FormulaElementDescription.SimpleRight)).floatValue = description.SimpleRight;   
                
                element.FindPropertyRelative(nameof(FormulaElementDescription.OperationType)).intValue = (int)description.OperationType;
            }
            
            formulaProperty.serializedObject.ApplyModifiedProperties();

            /*_usages = ParameterBuilderUsagesFactory.instance.Build(requiredId, _elements);

            var elementsMap = _usages.ToDictionary(x => x.ShortName, x => x.ParameterData.Id);
            var result = new List<HashedFormulaElement>();

            var pointer = 0;

          // ---

            // --

            var hashMap = result.ToDictionary(x => x.Hash);
            var sortedElements = result.Where(x => x.IsOperator() == true)
                .OrderByDescending(x => x.Weight).ToArray();

            var rawDescriptions = new List<FormulaElementDescription>();

            foreach (var hashedOperator in sortedElements)
            {
                var description = new FormulaElementDescription
                {
                    Hash = hashedOperator.Hash,
                    OperationType = hashedOperator.MathOperationType
                };

                var leftHash = hashedOperator.Left;
                var rightHash = hashedOperator.Right;

                if (leftHash != 0 && hashMap.TryGetValue(leftHash, out var left) == true)
                {
                    description.LeftIndex = TryGetReferenceIndex(rawDescriptions, operators, hashedOperator, left);

                    if (description.LeftIndex == -1)
                    {
                        description.SimpleLeft = left.SimpleValue;
                        description.LeftParameterId = left.ParameterId;
                    }

                    if (description.LeftIndex != -1)
                        description.LeftSource = FormulaDataSource.OtherDescriptionValue;
                    else if (description.LeftParameterId != 0)
                        description.LeftSource = FormulaDataSource.Parameter;
                    else
                        description.LeftSource = FormulaDataSource.SimpleValue;
                }

                if (hashedOperator.Right != 0 && hashMap.TryGetValue(rightHash, out var right) == true)
                {
                    description.RightIndex = TryGetReferenceIndex(rawDescriptions, operators, hashedOperator, right);

                    if (description.RightIndex == -1)
                    {
                        description.SimpleRight = right.SimpleValue;
                        description.RightParameterId = right.ParameterId;
                    }

                    if (description.RightIndex != -1)
                        description.RightSource = FormulaDataSource.OtherDescriptionValue;
                    else if (description.RightParameterId != 0)
                        description.RightSource = FormulaDataSource.Parameter;
                    else
                        description.RightSource = FormulaDataSource.SimpleValue;
                }

                rawDescriptions.Add(description);
            }

            Descriptions = rawDescriptions.ToArray();*/
        }

        private static void SetCorrectRightReferences(List<HashedFormulaElement> operators, List<HashedFormulaElement> result)
        {
            for (int i = 0; i < operators.Count; i++)
            {
                var index = result.IndexOf(operators[i]) + 1;

                for (int j = index; j < result.Count; j++)
                {
                    if (result[j].IsOpenGroup() == true || result[j].IsCloseGroup() == true)
                        continue;

                    operators[i].Right = result[j].Hash;
                    break;
                }
            }
        }

        private static int TryGetReferenceIndex(List<FormulaElementDescription> rawDescriptions,
            List<HashedFormulaElement> operators,
            HashedFormulaElement source,
            HashedFormulaElement element)
        {
            var referenceOperator = operators
                .FirstOrDefault(x =>
                    x.Left == element.Hash && source.Hash != x.Hash ||
                    x.Right == element.Hash && source.Hash != x.Hash);

            if (referenceOperator != null)
            {
                var index = rawDescriptions.FindIndex(x => x.Hash == referenceOperator.Hash);

                while (index != -1 && rawDescriptions.Any(x => x.LeftIndex == index || x.RightIndex == index) == true)
                {
                    var existElement = rawDescriptions.First(x => x.LeftIndex == index || x.RightIndex == index);
                    index = rawDescriptions.IndexOf(existElement);
                }

                return index;
            }

            return -1;
        }
    }
}