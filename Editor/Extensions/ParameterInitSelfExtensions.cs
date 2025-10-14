using System;
using System.Collections.Generic;
using System.Linq;

namespace Parameters.Editor.Extensions
{
    public static class ParameterInitSelfExtensions
    {
        private static readonly Dictionary<Type, string> Cache = new();
        
        public static string GetDisplayName(this Type type)
        {
            if (Cache.ContainsKey(type) == true)
                return Cache[type];
            
            var customAttributes = type.GetCustomAttributesData();
            
            var name = (string)customAttributes.First(x => x.AttributeType.Name == "ParameterInitSelfAttribute")
                .ConstructorArguments[0]
                .Value;

            if (string.IsNullOrEmpty(name) == false)
                return name;

            Cache.Add(type, name);
            return name;
        }
    }
}