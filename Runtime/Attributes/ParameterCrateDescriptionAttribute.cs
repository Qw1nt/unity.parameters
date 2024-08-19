using System;

namespace Parameters.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParameterCrateDescriptionAttribute : Attribute
    {
        public ParameterCrateDescriptionAttribute(Type type, string componentName = null)
        {
            
        }
    }
}