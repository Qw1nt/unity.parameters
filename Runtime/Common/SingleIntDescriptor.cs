namespace Parameters.Runtime.Common
{
    public struct SingleIntDescriptor
    {
        public int Value;

        public static implicit operator float(SingleIntDescriptor descriptor)
        {
            return descriptor.Value;
        }
    }

    public static class SingleIntDescriptorExtensions
    {
        public static SingleIntDescriptor SetValue(this SingleIntDescriptor descriptor, double value)
        {
            descriptor.Value = (int)value;
            return descriptor;
        }
    }
}