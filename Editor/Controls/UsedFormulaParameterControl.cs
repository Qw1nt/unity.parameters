using Parameters.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Parameters.Editor.Controls
{
    public class UsedFormulaParameterControl : VisualElement
    {
        private readonly PropertyField _parameterIdField;
        private readonly TextField _nameField;
        
        public UsedFormulaParameterControl()
        {
            _parameterIdField = new PropertyField()
                .AddTo(this);

            _nameField = new TextField("Short name:")
                .AddTo(this);
        }

        public UsedFormulaParameterControl Bind(SerializedProperty property)
        {
            _parameterIdField.Bind(property.FindPropertyRelative("_parameterIdProvider"));
            _nameField.Bind(property.FindPropertyRelative("_shortName"));

            return this;
        }
    }
}