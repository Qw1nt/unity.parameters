namespace Parameters.Runtime.Interfaces
{
    public interface IReadOnlyParameter
    {
        public float GetCleanRawValue();

        public float GetRawValue();

        public float GetCleanRawOverallValue();

        public float GetRawOverallValue();
    }
}