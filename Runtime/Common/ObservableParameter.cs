namespace Parameters.Runtime.Common
{
    public readonly struct ObservableParameter
    {
        internal ObservableParameter(ComplexParameter value)
        {
            Value = value;
        }

        internal readonly ComplexParameter Value;
    }

    public readonly struct ObservableParameter<T>
        where T : class
    {
        internal ObservableParameter(ComplexParameter value, T data)
        {
            Value = value;
            Data = data;
        }

        internal readonly ComplexParameter Value;
        internal readonly T Data;
    }
}