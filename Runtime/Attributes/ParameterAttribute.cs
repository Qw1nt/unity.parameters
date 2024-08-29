using System;

namespace Parameters.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Struct)]
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(Type type)
        {
            
        }
    }
}