using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using Parameters.Editor.Windows;
using Parameters.Runtime.Common;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Inspectors
{
    [CustomPropertyDrawer(typeof(ParameterBuilder))]
    public class ParameterBuilderPropertyDrawer : PropertyDrawer
    {
        
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();

            var idProperty = property.FindPropertyRelative("_parameterId");

            var parameterFieldContainer = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(1f)
                .AlignItems(Align.Center)
                .JustifyContent(Justify.SpaceBetween)
                .AddTo(root);
            
            parameterFieldContainer.Add(new Label("Parameter: "));

            var idButton  = new Button()
                .FlexGrow(1f)
                .AddTo(parameterFieldContainer);
            
            idButton.RegisterCallback<ClickEvent, ParameterBuilderPropertyDrawer>((evt, self) =>
            {
                var window = ScriptableObject.CreateInstance<SearchParameterWindow>();
                
                window.SetSelectCallback(val =>
                {
                    idProperty.intValue = val;
                    idProperty.serializedObject.ApplyModifiedProperties();
                    UpdateButtonText(idButton, idProperty);
                });
                
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), window);
            }, this);

            UpdateButtonText(idButton, idProperty);
            
            return root;
        }

        private void UpdateButtonText(Button button, SerializedProperty idProperty)
        {
            button.SetText(idProperty.intValue == 0
                ? "Select parameter"
                : DatabaseUtils.instance.GetParameterName(idProperty.intValue));
        }
    }
}