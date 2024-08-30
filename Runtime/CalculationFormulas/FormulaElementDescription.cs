using System;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    public struct FormulaElementDescription
    {
        public string Expression;

        public int LeftIndex;
        public int RightIndex;
        
        public ulong LeftParameterId;
        public ulong RightParameterId;
        
        public float SimpleLeft;
        public float SimpleRight;
    }
}