using System;

namespace Parameters.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParameterAttribute : Attribute
    {
        public ParameterAttribute(Type type)
        {
            
        }
    }
}