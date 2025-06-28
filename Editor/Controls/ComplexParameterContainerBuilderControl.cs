using System;
using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using Parameters.Editor.Windows;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.Controls
{
    public class ComplexParameterContainerBuilderControl : VisualElement
    {
        private readonly Label _propertyNameLabel;
        private readonly ListView _listView;
        private readonly VisualElement _expandableRoot;

        private SerializedProperty _parameters;

        private static StyleSheet _listStyle;

        public ComplexParameterContainerBuilderControl(bool showPropertyNameLabel = true)
        {
            _propertyNameLabel = new Label()
                .AddTo(this);

            _expandableRoot = new VisualElement()
                .AddTo(this);

            _listView = new ListView()
                .HideSizeCounter()
                .SelectionType(SelectionType.None)
                .VirtualizationMethod(CollectionVirtualizationMethod.DynamicHeight)
                .AddTo(_expandableRoot);

            _listStyle ??= Resources.Load<StyleSheet>("Uss/ParametersListViewStyle");

            _listView.makeItem = () => new ListViewItem();
            _listView.bindItem = (element, index) => ((ListViewItem)element).Bind(_parameters, index);
            _listView.reorderable = false;
            _listView.allowAdd = false;
            _listView.allowRemove = false;
            _listView.styleSheets.Add(_listStyle);

            var addElementButton = new Button()
                .SetText("+")
                .FontSize(14)
                .JustifyContent(Justify.Center)
                .FlexGrow(1)
                .FlexDirection(FlexDirection.Row)
                .AddTo(this);

            addElementButton.clicked += ShowTypesWindow;
        }
        
        public ComplexParameterContainerBuilderControl Bind(SerializedProperty property)
        {
            _propertyNameLabel.text = property.displayName;

            _parameters = property.FindPropertyRelative("_parameters");
            _listView.Bind(_parameters);

            return this;
        }

        private void ShowTypesWindow()
        {
            var window = ScriptableObject.CreateInstance<SearchParameterWindow>();
            
            window.SetSelectCallback(id =>
            {
                _parameters.arraySize++;
                
                var element = _parameters.GetArrayElementAtIndex(_parameters.arraySize - 1);
                element.ClearPropertyBuilder();
                element.FindPropertyRelative("_parameterId").FindPropertyRelative("_id").intValue = id;

                _parameters.serializedObject.ApplyModifiedProperties();
            });

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), window);
        }

        private class ListViewItem : VisualElement
        {
            private SerializedProperty _parameters;
            private int _elementIndex;

            public readonly ParameterBuilderCard Card;
            public readonly Button DeleteElementButton;

            public ListViewItem()
            {
                var root = new VisualElement()
                    .Margin(12f, 0f, 2f, 2f)
                    .FlexDirection(FlexDirection.Row)
                    .AddTo(this);

                Card = new ParameterBuilderCard(false)
                    .FlexGrow(1f)
                    .AddTo(root);

                DeleteElementButton = EditorUIElementFactory
                    .CreateDeleteButton()
                    .AddTo(root);

                DeleteElementButton.RegisterCallback<ClickEvent, ListViewItem>((_, self) =>
                {
                    self._parameters.DeleteArrayElementAtIndex(self._elementIndex);
                    self._parameters.serializedObject.ApplyModifiedProperties();
                }, this);
            }

            public void Bind(SerializedProperty parameters, int index)
            {
                _parameters = parameters;
                _elementIndex = index;

                Card.Bind(_parameters.GetArrayElementAtIndex(index));
            }
        }
    }
}