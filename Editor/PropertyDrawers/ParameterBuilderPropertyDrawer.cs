using Parameters.Runtime.Common;
using Plugins.unity.parameters.Editor.Controls;
using UnityEditor;
using UnityEngine.UIElements;

namespace Parameters.Editor.Inspectors
{
    [CustomPropertyDrawer(typeof(ParameterBuilder))]
    public class ParameterBuilderPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new ParameterBuilderCard(property);
        }
    }
}