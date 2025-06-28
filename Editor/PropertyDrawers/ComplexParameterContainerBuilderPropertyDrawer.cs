using Parameters.Editor.Controls;
using Parameters.Runtime.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace Parameters.Editor.PropertyDrawers
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