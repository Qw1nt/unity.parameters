using System;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    public class HashedFormulaElement
    {
        public string Expression;
        public ulong Hash;
        public uint Weight;
        public int Position;

        public FormulaItem Type;

        public ulong ParameterId;
        public float SimpleValue;

        public ulong Left;
        public ulong Right;
    }
}