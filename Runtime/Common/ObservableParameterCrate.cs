namespace Parameters.Runtime.Common
{
    public readonly struct ObservableParameterCrate
    {
        internal ObservableParameterCrate(IParameterCrate value)
        {
            Value = value;
        }
        
        internal readonly IParameterCrate Value;
    }
}