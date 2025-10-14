namespace Parameters.Runtime.Common
{
    public readonly struct StaticId
    {
        public readonly int Value;
        public readonly bool HasValue;

        public StaticId(int value)
        {
            Value = value;
            HasValue = true;
        }
    }
}