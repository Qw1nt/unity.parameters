namespace Parameters.Runtime.Common
{
    internal readonly struct StaticId
    {
        public readonly ulong Value;
        public readonly bool HasValue;

        public StaticId(ulong value)
        {
            Value = value;
            HasValue = true;
        }
    }
}