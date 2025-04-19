using Parameters.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Inspectors
{
    [CustomPropertyDrawer(typeof(ParameterBuilder), true)]
    public class ParameterBuilderPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Debug.Log("Abibass");
            
            // Корневой контейнер
            var root = new VisualElement();

            // Поля
            var parameterField    = new PropertyField(property.FindPropertyRelative("_parameter"),   "Parameter");
            var withDefaultField  = new PropertyField(property.FindPropertyRelative("_withDefaultValue"), "Use Default");
            var flatValueField    = new PropertyField(property.FindPropertyRelative("_flatValue"),    "Flat Value");
            var percentValueField = new PropertyField(property.FindPropertyRelative("_percentValue"), "Percent Value");
            var formulaField      = new PropertyField(property.FindPropertyRelative("_formula"),      "Formula");

            // Добавляем в корень
            root.Add(parameterField);
            root.Add(withDefaultField);
            root.Add(flatValueField);
            root.Add(percentValueField);
            root.Add(formulaField);

            void UpdateVisibility()
            { 
                bool useDef = property.FindPropertyRelative("_withDefaultValue").boolValue;
                flatValueField.style.display = useDef ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Подписываемся на изменение
            property.serializedObject.Update();
            UpdateVisibility();
            withDefaultField.RegisterCallback<ChangeEvent<bool>>(evt =>
            {
                property.FindPropertyRelative("_withDefaultValue").boolValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
                UpdateVisibility();
            });

            return root;
        }
    }
}