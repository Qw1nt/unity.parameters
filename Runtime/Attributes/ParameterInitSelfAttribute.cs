using System;

namespace Parameters.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ParameterInitSelfAttribute : Attribute
    {
        public ParameterInitSelfAttribute(string displayFullName)
        {
        }
    }
}