using System;
using Parameters.Runtime.Common;
using UnityEditor;

namespace Parameters.Editor.Extensions
{
    internal static class ParameterBuilderExtensions
    {
        public static void ClearPropertyBuilder(this SerializedProperty property)
        {
            if (property.boxedValue is ParameterBuilder builder == false)
                throw new ArgumentException();

            property.FindPropertyRelative("_parameterId").FindPropertyRelative("_id").intValue = 0;
            property.FindPropertyRelative("_flatValue").floatValue = 1f;
            property.FindPropertyRelative("_percentValue").floatValue = 1f;

            var formula = property.FindPropertyRelative("_formula");
            formula.FindPropertyRelative("_elements").ClearArray();
            formula.FindPropertyRelative("_usages").ClearArray();
            formula.FindPropertyRelative("Descriptions").ClearArray();
            formula.FindPropertyRelative("Dependencies").ClearArray();
            
            formula.FindPropertyRelative("_formula").stringValue = string.Empty;
            
        }
    }
}