using Parameters.Editor.Extensions;
using Parameters.Editor.Windows;
using Parameters.Runtime.Common;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(ParameterIdProvider))]
    internal class ParameterIdProviderPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var idProperty = property.FindPropertyRelative("_id");

            var parameterFieldContainer = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(1f)
                .AlignItems(Align.Center)
                .JustifyContent(Justify.SpaceBetween);

            parameterFieldContainer.Add(new Label("Parameter: "));

            var idButton = new Button()
                .FlexGrow(1f)
                .AddTo(parameterFieldContainer);

            var hiddenProp = new IntegerField()
                .Bind(idProperty)
                .Display(DisplayStyle.None)
                .AddTo(parameterFieldContainer);
            
            hiddenProp.RegisterValueChangedCallback(_ => idButton.DisplayParameterName(property));

            idButton.RegisterCallback<ClickEvent>(_ =>
            {
                var window = ScriptableObject.CreateInstance<SearchParameterWindow>();

                window.SetSelectCallback(val => { hiddenProp.value = val; });

                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    window);
            });
            
            return parameterFieldContainer;
        }
    }
}