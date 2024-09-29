namespace Parameters.Runtime.CalculationFormulas
{
#if UNITY_EDITOR
    public class FormulaElementMeta
    {
        public string Symbol;
        public ulong Hash;
        public uint Weight;
        public FormulaOperation Operation;
        
        public FormulaElementMeta(string symbol, ulong hash, uint weight, FormulaOperation operation)
        {
            Symbol = symbol;
            Hash = hash;
            Weight = weight;
            Operation = operation;
        }
    }
#endif
}