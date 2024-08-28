using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Parameters.Runtime.CalculationFormulas
{
    [CreateAssetMenu]
    public class CalculationFormulaData : ScriptableObject
    {
        [SerializeField] private CalculationFormulaElement[] _elements;

        [TextArea] [SerializeField] private string _formula;

        [SerializeField] private FormulaElementDescription[] _descriptions;

        private void OnValidate()
        {
            var s = ParseToSimple(_formula);

            /*
            var formula = new string(_formula);
            var que = new Queue<string>();

            while (formula.Length > 0)
            {
                var openS = formula.IndexOf('(');
                var closeS = formula.IndexOf(')');

                if (openS == -1 || closeS == -1)
                    break;

                que.Enqueue(formula.Substring(openS + 1, closeS - openS - 1));
                formula = formula.Remove(openS, closeS - openS + 1);
            }
            */

            return;
            var operators = new HashSet<string>() { "+", "-" };
            var elementsMap = _elements.ToDictionary(x => x.ShortName);

            var descriptions = new List<FormulaElementDescription>();
            var parts = _formula.Split(' ').Where(x => string.IsNullOrEmpty(x) == false).ToList();

            var i = 0;
            while (parts.Count > 0)
            {
                if (operators.Contains(parts[i]) == false)
                {
                    i++;
                    continue;
                }

                if (i >= 1)
                {
                    var leftName = parts[i - 1];
                    var rightName = parts[i + 1];

                    if (elementsMap.ContainsKey(leftName) == false || elementsMap.ContainsKey(rightName) == false)
                        return;

                    descriptions.Add(new FormulaElementDescription
                    {
                        Id = BuildRandomId(),
                        Left = elementsMap[leftName].ParameterCrateData.Id,
                        Right = elementsMap[rightName].ParameterCrateData.Id,
                        Char = parts[i]
                    });

                    parts.RemoveAt(--i);
                    parts.RemoveAt(i);
                    parts.RemoveAt(i);
                }
                else if (descriptions.Count > 0 && parts.Count > i + 1)
                {
                    if (elementsMap.ContainsKey(parts[i + 1]) == false)
                        return;

                    descriptions.Add(new FormulaElementDescription()
                    {
                        Id = BuildRandomId(),
                        Left = descriptions[^1].Id,
                        Right = elementsMap[parts[i + 1]].ParameterCrateData.Id,
                        Char = parts[i]
                    });

                    parts.RemoveAt(i);
                    parts.RemoveAt(i);
                }
                else
                {
                    return;
                }
            }

            _descriptions = descriptions.ToArray();

            ulong BuildRandomId()
            {
                return (ulong)Random.Range(int.MaxValue / 2, int.MaxValue) *
                       (ulong)Random.Range(int.MaxValue / 2, int.MaxValue);
            }
        }

        private string[] ParseToSimple(string input)
        {
            var groupBuffer = new Queue<string>();
            var result = new List<string>();

            var groups = ParseGroups(input);

            var i = 0;

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

            return null;
        }

        private (int startIndex, int endIndex) GetGroupIndexes(string input, int startIndex)
        {
            var depth = 0;
            var i = startIndex;

            while (i < input.Length)
            {
                if (input[i] == '(')
                    depth++;

                if (input[i] == ')')
                    depth--;

                if (depth == 0)
                    return (startIndex, i);

                i++;
            }

            throw new ArgumentException();
        }

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

            for (int i = selectOperation.maxDepth - 1, j = 0; i >= 0; i--)
            {
                foreach (var groupInfo in sortedGroups[i])
                {
                    result[j] = groupInfo;
                    result[j].Index = j;
                    j++;
                }
            }

            var resultMap = result.Where(x => x.Depth > 0).ToDictionary(x => x.Hash);
            var chars = input.ToCharArray().ToList();
            
            for (int i = 0; i < resultMap.Count; i++)
            {
                var depth = 0;
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

                        if (depth > 0)
                        {
                            var lastIndex = j;
                            var hash = CalculateGroupHash(chars, firstIndex, lastIndex);

                            if (resultMap.TryGetValue(hash, out var info) == true)
                            {
                                chars.RemoveRange(firstIndex, lastIndex - firstIndex + 1);
                                chars.Insert(firstIndex, '}');
                                chars.Insert(firstIndex, $"{info.Index}"[0]);
                                chars.Insert(firstIndex, '{');
                                
                                break;
                            }
                        }
                    }
                }
            }
            
            var r = new string(chars.ToArray());
            return null;
        }

        private (GroupInfo[] groups, int maxDepth) SelectGroups(string input)
        {
            var depth = 0;
            var maxDepth = 0;
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
                var depth = 0;

                while (pointer < input.Length)
                {
                    if (input[pointer] == '(')
                        depth++;

                    if (input[pointer] == ')')
                        depth--;

                    if (depth == 0)
                    {
                        info.EndPosition = pointer;
                        info.InternalExpression = input.Substring(info.StartPosition + 1, pointer - info.StartPosition - 1);
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
        }

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