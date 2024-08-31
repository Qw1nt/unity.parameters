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
        
        public ulong ParameterId;
        public float SimpleValue;

        public ulong Left;
        public ulong Right;
        
        public FormulaDataSource NodeType;
        public FormulaOperation MathOperationType;
    }
}