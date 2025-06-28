using System.Collections.Generic;
using Parameters.Runtime.Common;
using Plugins.unity.parameters.Editor.Controls;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ComplexParameterContainerBuilder))]
    internal class ComplexParameterContainerBuilderPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new ComplexParameterContainerBuilderControl().Bind(property);
        }
    }
}