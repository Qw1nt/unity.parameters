using System;
using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using Parameters.Editor.Windows;
using Plugins.unity.parameters.Editor.Base;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Controls
{
    public class ComplexParameterContainerBuilderControl : ExpandableLazyInitElement
    {
        private readonly Label _propertyNameLabel;
        private readonly VisualElement _expandableRoot;
        private ListView _listView;

        private SerializedProperty _parameters;

        private static StyleSheet _listStyle;
        
        public ComplexParameterContainerBuilderControl(bool showPropertyNameLabel = true)
        {
            this.MarginTop(4f)
                .BorderRadius(8f)
                .Padding(2f)
                .BackgroundColor(new Color(0.2f, 0.2f, 0.2f));
            
            _propertyNameLabel = new Label()
                .FontSize(14f)
                .Padding(6f)
                .AddTo(this);

            _propertyNameLabel.RegisterCallback<ClickEvent, ComplexParameterContainerBuilderControl>((evt, self) =>
            {
                self.SwitchExpandState();
            }, this);

            _expandableRoot = new VisualElement()
                .AddTo(this);
        }
        
        protected override VisualElement ExpandElementsContainer => _expandableRoot;

        protected override void Initialize()
        {
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

            _listView.Bind(_parameters);

            var addElementButton = new Button()
                .SetText("+")
                .FontSize(14)
                .JustifyContent(Justify.Center)
                .FlexGrow(1)
                .FlexDirection(FlexDirection.Row)
                .AddTo(_expandableRoot);

            addElementButton.clicked += ShowTypesWindow;
        }
        
        public ComplexParameterContainerBuilderControl Bind(SerializedProperty property)
        {
            _propertyNameLabel.text = property.displayName;

            _parameters = property.FindPropertyRelative("_parameters");
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