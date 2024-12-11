using System;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    public struct FormulaElementDescription
    {
        public ulong Hash;

        public FormulaDataSource LeftSource;
        public FormulaDataSource RightSource;
        
        public int LeftIndex;
        public int RightIndex;
        
        public int LeftParameterId;
        public int RightParameterId;
        
        public float SimpleLeft;
        public float SimpleRight;

        public FormulaOperation OperationType;

        public float CalculatedValue;
    }
}