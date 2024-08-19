using System;
using System.Collections.Generic;
using Parameters.Runtime.Attributes;
using UnityEditor;

namespace Parameters.Editor
{
    public class EditorParametersStorage
    {
        private static readonly Dictionary<Type, string> Map = new();
        
        static EditorParametersStorage()
        {
            var collection = TypeCache.GetTypesWithAttribute(typeof(ParameterCrateDescriptionAttribute));

            foreach (var type in collection)
                Map.Add(type, type.Name);
        }

        public static IReadOnlyDictionary<Type, string> Data => Map;
    }
}