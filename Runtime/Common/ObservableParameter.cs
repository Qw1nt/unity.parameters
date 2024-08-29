namespace Parameters.Runtime.Common
{
    public readonly struct ObservableParameter
    {
        internal ObservableParameter(Parameter value)
        {
            Value = value;
        }
        
        internal readonly Parameter Value;
    }
}