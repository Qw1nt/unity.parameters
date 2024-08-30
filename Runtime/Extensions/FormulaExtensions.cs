using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;

namespace Parameters.Runtime.Extensions
{
    internal static class FormulaExtensions
    {
        private const ulong Offset = 777_777_777_777_777_777UL;
        
        private static readonly Dictionary<char, ulong> ReservedSymbolsMap = new()
        {
            { Symbols.OpenGroup, 600_000_000UL },
            { Symbols.CloseGroup, 500_000_000UL },
            { Symbols.Multiply, (ulong)FormulaOperation.Multiply },
            { Symbols.Divide, (ulong)FormulaOperation.Divide },
            { Symbols.Add, (ulong)FormulaOperation.Add },
            { Symbols.Subtract, (ulong)FormulaOperation.Subtract },
        };

        private static readonly Dictionary<char, uint> CharWeightTable = new()
        {
            { Symbols.OpenGroup, 100_000u },
            { Symbols.CloseGroup, 100_000u },
            { Symbols.Multiply, 10_000u },
            { Symbols.Divide, 10_000u },
            { Symbols.Add, 1000u },
            { Symbols.Subtract, 1000u }
        };

        private static readonly Dictionary<ulong, uint> HashWeightTable = new()
        {
            { ReservedSymbolsMap[Symbols.OpenGroup], CharWeightTable[Symbols.OpenGroup] },
            { ReservedSymbolsMap[Symbols.CloseGroup], CharWeightTable[Symbols.CloseGroup] },
            { (ulong)FormulaOperation.Multiply, CharWeightTable[Symbols.Multiply] },
            { (ulong)FormulaOperation.Divide, CharWeightTable[Symbols.Divide] },
            { (ulong)FormulaOperation.Add, CharWeightTable[Symbols.Add] },
            { (ulong)FormulaOperation.Subtract, CharWeightTable[Symbols.Subtract] }
        };

        private static readonly Dictionary<char, FormulaOperation> OperationsMap = new()
        {
            {Symbols.Multiply, FormulaOperation.Multiply},
            {Symbols.Divide, FormulaOperation.Divide},
            {Symbols.Add, FormulaOperation.Add},
            {Symbols.Subtract, FormulaOperation.Subtract},
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReserved(this char symbol)
        {
            return ReservedSymbolsMap.ContainsKey(symbol);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReserved(this ulong hash)
        {
            return ReservedSymbolsMap.ContainsValue(hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOperator(this HashedFormulaElement element)
        {
            return element.GetNormalizedElementHash() switch
            {
                (ulong)FormulaOperation.Multiply => true,
                (ulong)FormulaOperation.Divide => true,
                (ulong)FormulaOperation.Subtract => true,
                (ulong)FormulaOperation.Add => true,
                _ => false
            };
        }
        
        public static bool IsOpenGroup(this HashedFormulaElement element)
        {
            return element.GetNormalizedElementHash() == ReservedSymbolsMap[Symbols.OpenGroup];
        }
        
        public static bool IsCloseGroup(this HashedFormulaElement element)
        {
            return element.GetNormalizedElementHash() == ReservedSymbolsMap[Symbols.CloseGroup];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetReservedCharHash(this char symbol)
        {
            return ReservedSymbolsMap[symbol];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetSymbolWeight(this HashedFormulaElement element)
        {
            return HashWeightTable[element.GetNormalizedElementHash()];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FormulaOperation GetOperationType(this HashedFormulaElement element)
        {
            return OperationsMap[element.Expression[0]];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetRawHash(this string input, int startPosition, out int endPosition)
        {
            endPosition = startPosition;

            while (char.IsWhiteSpace(input[endPosition]) == true)
            {
                endPosition++;

                if (endPosition < input.Length)
                    continue;

                endPosition = -1;
                return 0;
            }

            var firstChar = input[endPosition];

            if (ReservedSymbolsMap.ContainsKey(firstChar) == true)
            {
                endPosition++;
                return firstChar.GetReservedCharHash();
            }

            var pointer = 0;
            var hash = 0UL;

            while (char.IsWhiteSpace(input[endPosition]) == false)
            {
                hash += (ulong)firstChar.GetHashCode();

                if (++pointer % 2 == 0)
                    hash *= Offset;

                endPosition++;

                if (input[endPosition].IsReserved() == true)
                    return hash;

                if (endPosition < input.Length)
                    continue;

                if (char.IsWhiteSpace(input[^1]) == false)
                {
                    endPosition = startPosition;
                    return hash;
                }
            }
            
            endPosition++;
            return hash;
        }

        public static void AdjustElementHash(this HashedFormulaElement element)
        {
            element.Hash += (ulong)element.Position;
        }

        public static ulong GetNormalizedElementHash(this HashedFormulaElement element)
        {
            return element.Hash - (ulong)element.Position;
        }
    }
}