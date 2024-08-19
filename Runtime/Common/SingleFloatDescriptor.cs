namespace Parameters.Runtime.Common
{
    public struct SingleFloatDescriptor
    {
        public float Value;
        
        public static implicit operator float (SingleFloatDescriptor descriptor)
        {
            return descriptor.Value;
        } 
    }

    public static class SingleFloatDescriptorExtensions
    {
        public static SingleFloatDescriptor SetValue(this SingleFloatDescriptor descriptor, double value)
        {
            descriptor.Value = (float) value;
            return descriptor;
        }
    }
}