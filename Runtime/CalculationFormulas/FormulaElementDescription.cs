using System;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    public struct FormulaElementDescription
    {
        public ulong Id;
        public string Char;
        public ulong Left;
        public ulong Right;
    }
}