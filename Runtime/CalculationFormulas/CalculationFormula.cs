using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using Parameters.Runtime.Extensions;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    internal class CalculationFormula
    {
#if UNITY_EDITOR
        [SerializeField] private List<CalculationFormulaElement> _elements;
        [ReadOnly] [SerializeField] private List<CalculationFormulaElement> _usages;
        [TextArea] [SerializeField] private string _formula;
#endif

        [ReadOnly] public FormulaElementDescription[] Descriptions;
        [ReadOnly] public ulong[] Dependencies;

#if UNITY_EDITOR

        public void Prepare(ParameterData required)
        {
            if (string.IsNullOrEmpty(_formula) == true)
            {
                _usages = null;
                Descriptions = null;
                Dependencies = null;
                return;
            }

            Dependencies = _elements.Select(x => x.ParameterData.Id).ToArray();
            _usages = ParameterBuilderUsagesFactory.instance.Build(required, _elements);
            
            var elementsMap = _elements.ToDictionary(x => x.ShortName, x => x.ParameterData.Id);
            var result = new List<HashedFormulaElement>();

            var pointer = 0;

            if (char.IsWhiteSpace(_formula[^1]) == false)
                _formula += ' ';

            while (pointer < _formula.Length)
            {
                var hash = _formula.GetRawHash(pointer, out var newPointer);

                if (newPointer == -1)
                    break;

                var item = new HashedFormulaElement
                {
                    Expression = _formula.Substring(pointer, newPointer - pointer).Replace(" ", ""),
                    Hash = hash,
                    Position = newPointer,
                };

                item.AdjustElementHash();
                result.Add(item);

                if (float.TryParse(item.Expression, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var simpleValue) == true)
                    item.SimpleValue = simpleValue;
                else if (elementsMap.TryGetValue(item.Expression, out var parameterId) == true)
                    item.ParameterId = parameterId;

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

            Descriptions = rawDescriptions.ToArray();
        }

        private void SetCorrectRightReferences(List<HashedFormulaElement> operators, List<HashedFormulaElement> result)
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

        private int TryGetReferenceIndex(List<FormulaElementDescription> rawDescriptions,
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
#endif
    }
}