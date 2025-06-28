using Parameters.Editor.Controls;
using Parameters.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace Parameters.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ParameterBuilder))]
    internal class ParameterBuilderPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new ParameterBuilderCard().Bind(property);
        }
    }
}