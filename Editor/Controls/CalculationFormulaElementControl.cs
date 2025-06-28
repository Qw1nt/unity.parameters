using Parameters.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Parameters.Editor.Controls
{
    public class CalculationFormulaElementControl : VisualElement
    {
        private readonly PropertyField _id;
        private readonly TextField _value;
        
        public CalculationFormulaElementControl()
        {
            _id = new PropertyField()
                .AddTo(this);
            
            _value = new TextField("Short name:")
                .AddTo(this);

            this.Padding(4f);
        }

        public void Bind(SerializedProperty elementProperty)
        {
            _id.BindProperty(elementProperty.FindPropertyRelative("_parameterIdProvider"));
            _value.BindProperty(elementProperty.FindPropertyRelative("_shortName"));
        }
    }
}