using Parameters.Editor.Controls;
using Parameters.Runtime.CalculationFormulas;
using UnityEditor;
using UnityEngine.UIElements;

namespace Parameters.Editor.PropertyDrawers
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