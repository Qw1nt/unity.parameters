namespace Parameters.Runtime.Interfaces
{
    public interface IParameterRef
    {
        public ulong ParameterId { get; }
        
        void SetValue(double value);

        float GetValue();
    }
}