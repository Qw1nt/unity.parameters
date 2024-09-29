using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;

namespace Parameters.Runtime.Extensions
{
    internal static class FormulaExtensions
    {
        private const ulong Offset = 777_777_777_777_777_777UL;

        private static readonly List<FormulaElementMeta> Meta = new()
        {
            new FormulaElementMeta(Symbols.OpenGroup, 600_000_000UL, 100_000u, FormulaOperation.None),
            new FormulaElementMeta(Symbols.CloseGroup, 500_000_000UL, 100_000u, FormulaOperation.None),
            new FormulaElementMeta(Symbols.Multiply, (ulong)FormulaOperation.Multiply, 10_000u, FormulaOperation.Multiply),
            new FormulaElementMeta(Symbols.Divide, (ulong)FormulaOperation.Divide, 10_000u, FormulaOperation.Divide),
            new FormulaElementMeta(Symbols.Add, (ulong)FormulaOperation.Add, 1000u, FormulaOperation.Add),
            new FormulaElementMeta(Symbols.Subtract, (ulong)FormulaOperation.Subtract, 1000u, FormulaOperation.Subtract),
        };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReserved(this char symbol)
        {
            return Meta.Any(x => x.Symbol[0] == symbol);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReserved(this string symbol)
        {
            return Meta.Any(x => x.Symbol == symbol);
        }  
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReserved(this ulong hash)
        {
            return Meta.Any(x => x.Hash == hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOperator(this HashedFormulaElement element)
        {
            var normalizedElementHash = element.GetNormalizedElementHash();
            return Meta.Any(x => x.Hash == normalizedElementHash && x.Operation != FormulaOperation.None);
        }

        public static bool IsOpenGroup(this HashedFormulaElement element)
        {
            return Meta.Any(x => x.Hash == element.GetNormalizedElementHash() && x.Symbol == Symbols.OpenGroup);
        }

        public static bool IsCloseGroup(this HashedFormulaElement element)
        {
            return Meta.Any(x => x.Hash == element.GetNormalizedElementHash() && x.Symbol == Symbols.CloseGroup);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetReservedCharHash(this char symbol)
        {
            return Meta.First(x => x.Symbol[0] == symbol).Hash;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetReservedCharHash(this string symbol)
        {
            return Meta.First(x => x.Symbol == symbol).Hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetSymbolWeight(this HashedFormulaElement element)
        {
            return Meta.First(x => x.Hash == element.GetNormalizedElementHash()).Weight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FormulaOperation GetOperationType(this HashedFormulaElement element)
        {
            return Meta.First(x => x.Symbol == element.Expression).Operation;
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

            if (Meta.Any(x => x.Symbol[0] == firstChar) == true)
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