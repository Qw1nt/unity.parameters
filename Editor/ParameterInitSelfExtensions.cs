using System;
using System.Linq;

namespace Parameters.Editor
{
    public static class ParameterInitSelfExtensions
    {
        public static string GetDisplayName(this Type type)
        {
            var customAttributes = type.GetCustomAttributesData();
            return (string)customAttributes.First(x => x.AttributeType.Name == "ParameterInitSelfAttribute")
                .ConstructorArguments[0]
                .Value;
        }
    }
}