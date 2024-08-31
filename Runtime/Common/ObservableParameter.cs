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

    public readonly struct ObservableParameter<T>
        where T : class
    {
        internal ObservableParameter(Parameter value, T data)
        {
            Value = value;
            Data = data;
        }

        internal readonly Parameter Value;
        internal readonly T Data;
    }
}