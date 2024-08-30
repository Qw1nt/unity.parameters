using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Parameters.Runtime.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Parameters.Runtime.CalculationFormulas
{
    [CreateAssetMenu]
    public class CalculationFormulaData : ScriptableObject
    {
        private const int MinDepth = 0;

        [SerializeField] private CalculationFormulaElement[] _elements;

        [TextArea] [SerializeField] private string _formula;

        [SerializeField] private HashedFormulaElement[] _hashedElements;
        [SerializeField] private HashedFormulaElement[] _sorted;
        [SerializeField] private FormulaElementDescription[] _descriptions;

        private Dictionary<string, ulong> _elementsMap;

        private void OnValidate()
        {
            _elementsMap = _elements.ToDictionary(x => x.ShortName, x => x.ParameterData.Id);
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
                    Left = result.Count > 0 ? result[^1].Hash : 0UL,
                };

                item.AdjustElementHash();
                result.Add(item);

                if (float.TryParse(item.Expression, NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var simpleValue) == true)
                {
                    item.Type = FormulaItem.SimpleValue;
                    item.SimpleValue = simpleValue;
                }
                else if (_elementsMap.ContainsKey(item.Expression) == true)
                {
                    item.Type = FormulaItem.Parameter;
                    item.ParameterId = _elementsMap[item.Expression];
                }
                else
                {
                    item.Type = FormulaItem.ComplexValue;
                }

                if (result.Count > 1)
                    result[^2].Right = result[^1].Hash;

                pointer = newPointer;
            }

            var operators = result
                .Where(x => x.IsOperator() == true)
                .ToList();

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

                item.Weight = weight + item.Hash.GetSymbolWeight();
            }

            _hashedElements = result.ToArray();
            _sorted = result.Where(x => x.IsOperator() == true).OrderByDescending(x => x.Weight).ToArray();

            foreach (var element in _sorted)
            {
            }
        }

        private string[] ParseToSimple(string input)
        {
            // var groupBuffer = new Queue<string>();
            // var result = new List<string>();

            /*var groups = ParseGroups(input);
            var preparedExpression = groups[^1].InternalExpression;

            _descriptions = new FormulaElementDescription[groups.Length];

            for (int i = 0; i < groups.Length; i++)
            {
                _descriptions[i] = new FormulaElementDescription()
                {
                    Char = groups[i].InternalExpression
                };
            }*/

            // var items = preparedExpression.Split(' ');
            // _d = items;

            return null;

            /*var i = 0;

            while (i < input.Length)
            {
                if (input[i] == '(')
                {
                    var indexes = GetGroupIndexes(input, i);
                    groupBuffer.Enqueue(input.Substring(indexes.startIndex + 1, indexes.endIndex - 1));

                    i = indexes.endIndex;
                    continue;
                }

                i++;
            }

            while (groupBuffer.Count > 0)
            {
                var group = groupBuffer.Dequeue();
            }

            return null;*/
        }

        /*private int GetWeight()
        {

        }*/

        /*
        private GroupInfo[] ParseGroups(string input)
        {
            var selectOperation = SelectGroups(input);
            FindGroupsEnds(selectOperation.groups, input);

            var sortedGroups = new Dictionary<int, List<GroupInfo>>();

            foreach (var info in selectOperation.groups)
            {
                if (sortedGroups.ContainsKey(info.Depth) == false)
                    sortedGroups.Add(info.Depth, new List<GroupInfo>());

                sortedGroups[info.Depth].Add(info);
            }

            var result = new GroupInfo[sortedGroups.Sum(x => x.Value.Count)];

            for (int i = selectOperation.maxDepth - 1, j = 0; i >= MinDepth; i--)
            {
                foreach (var groupInfo in sortedGroups[i])
                {
                    result[j] = groupInfo;
                    result[j].Index = j;
                    j++;
                }
            }
            */

            /*var resultList = result.ToList();

            for (int i = 0; i < result.Length; i++)
            {
                ref var group = ref result[i];
                var hash = group.Hash;
                var expression = group.InternalExpression;

                if (resultList.Any(x => x.InternalExpression.Contains(expression) && x.Hash != hash) == true)
                {
                    var index = resultList.IndexOf(result.First(x => x.InternalExpression.Contains(expression) && x.Hash != hash));
                    result[index].InternalExpression = result[index].InternalExpression.Replace($"({expression})", $"{{{group.Index}}}");
                }
            }*/

            /*var resultMap = result.Where(x => x.Depth >= MinDepth).ToDictionary(x => x.Hash);
            var chars = input.ToCharArray().ToList();

            for (int i = 0; i < resultMap.Count; i++)
            {
                var depth = MinDepth;
                var firstIndex = 0;

                for (int j = 0; j < chars.Count; j++)
                {
                    var symbol = chars[j];

                    if (symbol == '(')
                    {
                        firstIndex = j;
                        depth++;
                    }

                    if (symbol == ')')
                    {
                        depth--;

                        if (depth >= 0)
                        {
                            var lastIndex = j;
                            var hash = CalculateGroupHash(chars, firstIndex, lastIndex);

                            if (resultMap.TryGetValue(hash, out var info) == true)
                            {
                                chars.RemoveRange(firstIndex, lastIndex - firstIndex + 1);
                                chars.Insert(firstIndex, '}');
                                chars.Insert(firstIndex, $"{info.Index}"[0]);
                                chars.Insert(firstIndex, '{');

                                result[info.Index] = info;

                                break;
                            }
                        }
                    }
                }
            }

            var lastExpression = new string(chars.ToArray());
            result[^1].InternalExpression = lastExpression;

            return result;
        }

        private (GroupInfo[] groups, int maxDepth) SelectGroups(string input)
        {
            var depth = MinDepth;
            var maxDepth = MinDepth;
            var stringPointer = 0;

            var infos = new List<GroupInfo>();

            while (stringPointer < input.Length)
            {
                if (input[stringPointer] == '(')
                {
                    depth++;
                    maxDepth = Math.Max(depth, maxDepth);

                    var info = new GroupInfo
                    {
                        Depth = depth - 1,
                        StartPosition = stringPointer
                    };

                    infos.Add(info);
                }

                if (input[stringPointer] == ')')
                    depth--;

                stringPointer++;
            }

            return (infos.ToArray(), maxDepth);
        }

        private void FindGroupsEnds(GroupInfo[] infos, string input)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                ref var info = ref infos[i];
                var pointer = info.StartPosition;
                var depth = MinDepth;

                while (pointer < input.Length)
                {
                    if (input[pointer] == '(')
                        depth++;

                    if (input[pointer] == ')')
                        depth--;

                    if (depth == MinDepth)
                    {
                        info.EndPosition = pointer;
                        info.InternalExpression =
                            input.Substring(info.StartPosition + 1, pointer - info.StartPosition - 1);
                        CalculateGroupHash(ref info, input);
                        break;
                    }

                    pointer++;
                }
            }
        }

        private void CalculateGroupHash(ref GroupInfo info, string input)
        {
            var hash = 0UL;
            var salt = 0;

            for (int i = info.StartPosition; i <= info.EndPosition; i++)
            {
                hash += (ulong)input[i].GetHashCode();

                if (++salt % 5 == 0)
                    hash *= 7_777_777_777_777_777UL;
            }

            info.Hash = hash;
        }

        private ulong CalculateGroupHash(List<char> chars, int startIndex, int endIndex)
        {
            var hash = 0UL;
            var salt = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                hash += (ulong)chars[i].GetHashCode();

                if (++salt % 5 == 0)
                    hash *= 7_777_777_777_777_777UL;
            }

            return hash;
        }*/

        private struct GroupInfo
        {
            public int StartPosition;
            public int EndPosition;
            public int Depth;
            public int Index;
            public ulong Hash;
            public string InternalExpression;
        }
    }
}