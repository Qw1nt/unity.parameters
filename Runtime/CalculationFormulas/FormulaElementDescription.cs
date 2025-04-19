using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Parameters.Runtime.CalculationFormulas
{
    [Serializable]
    public struct FormulaElementDescription : IEquatable<FormulaElementDescription>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(FormulaElementDescription other)
        {
            return LeftSource == other.LeftSource &&
                   RightSource == other.RightSource && 
                   LeftIndex == other.LeftIndex && 
                   RightIndex == other.RightIndex &&
                   LeftParameterId == other.LeftParameterId && 
                   RightParameterId == other.RightParameterId && 
                   Mathf.Approximately(SimpleLeft, other.SimpleLeft) &&
                   Mathf.Approximately(SimpleRight, other.SimpleRight) && 
                   OperationType == other.OperationType;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj is FormulaElementDescription other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (int)Hash;
        }
    }
}