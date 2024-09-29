namespace Parameters.Runtime.Interfaces
{
    public interface IReadOnlyParameter
    {
        public float GetCleanFlat();

        public float GetFlat();

        public float GetCleanPercent();

        public float GetPercent();
    }
}