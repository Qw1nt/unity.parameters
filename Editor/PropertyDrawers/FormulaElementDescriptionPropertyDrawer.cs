using Parameters.Runtime.CalculationFormulas;
using Plugins.unity.parameters.Editor.Controls;
using UnityEditor;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(FormulaElementDescription))]
    internal class FormulaElementDescriptionPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var result = new FormulaElementDescriptionControl();
            result.Bind(property);
            
            return result;
        }
    }
}