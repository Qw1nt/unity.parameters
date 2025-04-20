using System.Linq;
using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using Parameters.Editor.Windows;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.Controls
{
    public class ParameterBuilderCard : VisualElement
    {
        private static readonly Color BackgroundColor = new(0.22f, 0.22f, 0.22f);
        private static readonly Color HoverColor = new(0.39f, 0.39f, 0.39f);
        
        private readonly VisualElement _expandRoot;
        private readonly Label _title;

        private SerializedProperty _idProperty;
        private SerializedProperty _formula;
        private SerializedProperty _formulaDependencies;

        public ParameterBuilderCard(SerializedProperty property)
        {
            _idProperty = property.FindPropertyRelative("_parameterId");

            _title = new Label()
                .FontSize(14f)
                .Bold()
                .AddTo(this);

            this.Margin(2f)
                .Padding(6f)
                .BorderWidth(2f)
                .BorderRadius(8f)
                .BorderColor(new Color(0.37f, 0.37f, 0.37f));

            SetupTitle();

            _expandRoot = new VisualElement().MarginTop(16f).AddTo(this);

            BuildExpand(_expandRoot);
            BuildSettingsFields(property).AddTo(_expandRoot);
            BuildFormulaPart(property).AddTo(_expandRoot);

            _expandRoot.Display(DisplayStyle.None);
        }

        private void SetupTitle()
        {
            _title.RegisterCallback<ClickEvent, ParameterBuilderCard>(
                (_, self) => self._expandRoot.SwapDisplay(), this);

            _title.RegisterCallback<PointerEnterEvent, ParameterBuilderCard>(
                (_, self) => self.BackgroundColor(HoverColor), this);

            _title.RegisterCallback<PointerLeaveEvent, ParameterBuilderCard>(
                (_, self) => self.BackgroundColor(BackgroundColor), this);

            _title.DisplayParameterName(_idProperty);
        }

        private void BuildExpand(VisualElement expandRoot)
        {
            var parameterFieldContainer = new VisualElement()
                .FlexDirection(FlexDirection.Row)
                .FlexGrow(1f)
                .AlignItems(Align.Center)
                .JustifyContent(Justify.SpaceBetween)
                .AddTo(expandRoot);

            var prop = new PropertyField(_idProperty)
                .FlexGrow(1f)
                .AddTo(parameterFieldContainer);

            prop.RegisterValueChangeCallback(evt => _title.DisplayParameterName(evt.changedProperty));
        }

        private VisualElement BuildSettingsFields(SerializedProperty property)
        {
            var root = new VisualElement()
                .Margin(0f, 0f, 4f, 0f);

            var flat = new FloatField("Flat:")
                .Bind(property.FindPropertyRelative("_flatValue"))
                .AddTo(root);

            var percent = new FloatField("Percent:")
                .Bind(property.FindPropertyRelative("_percentValue"))
                .AddTo(root);

            return root;
        }

        private VisualElement BuildFormulaPart(SerializedProperty property)
        {
            _formula = property.FindPropertyRelative("_formula");
            _formulaDependencies = _formula.FindPropertyRelative("Dependencies");
            
            var root = new VisualElement()
                .MarginTop(16f);

            new Label("Formula")
                .Margin(0f, 0f, 0f, 8f)
                .FontSize(13f)
                .Bold()
                .BorderWidth(0f, 0f, 0f, 2f)
                .BorderColor(new Color(0.37f, 0.37f, 0.37f))
                .AddTo(root);

            new PropertyField(_formula.FindPropertyRelative("_elements"))
                .AddTo(root);

            var inputField = new TextField()
                .Bind(_formula.FindPropertyRelative("_formula"))
                .AddTo(root);

            inputField.multiline = true;

            var dependenciesViewLabel = new Label("Dependencies")
                .MarginTop(8f)
                .FontSize(11f)
                .Bold()
                .AddTo(root);
            
            var dependenciesView = new ListView()
                .HideSize()
                .ReadOnly()
                .Bind(_formulaDependencies)
                .AddTo(root);
            
            dependenciesView.makeItem = () =>
            {
                var elRoot = new VisualElement();
                new Label()
                    .MarginTop(2f)
                    .Padding(4f)
                    .BackgroundColor(new Color(0.19f, 0.19f, 0.19f))
                    .AddTo(elRoot);

                return elRoot;
            };
            dependenciesView.bindItem = (label, index) =>
            {
                ((Label)label.Children().First()).text = _formulaDependencies.GetArrayElementAtIndex(index).intValue.ToString();
            };
            
            var prepareButton = new Button()
                .SetText("PrepareFormula")
                .AddTo(root);

            prepareButton.RegisterCallback<ClickEvent, ParameterBuilderCard>(
                (_, self) => FormulaExtensions.Prepare(self._formula, _idProperty.FindPropertyRelative("_id").intValue),
                this);

            return root;
        }
    }
}