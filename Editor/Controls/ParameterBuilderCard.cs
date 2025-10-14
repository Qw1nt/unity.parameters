using Parameters.Editor.Extensions;
using Plugins.unity.parameters.Editor.Base;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Controls
{
    public class ParameterBuilderCard : ExpandableLazyInitElement
    {
        private static readonly Color BackgroundColor = new(0.22f, 0.22f, 0.22f);
        private static readonly Color HoverColor = new(0.39f, 0.39f, 0.39f);

        private readonly Label _displayNameLabel;
        private readonly VisualElement _root;

        private readonly VisualElement _expandRoot;
        private readonly Label _title;

        private FloatField _flatValueField;
        private FloatField _percentValueField;

        private CalculationFormulaControl _formulaControl;

        private SerializedProperty _idProperty;
        private SerializedProperty _formula;
        private SerializedProperty _formulaDependencies;
        private SerializedProperty _formulaUsages;

        private SerializedProperty _sourceBindProperty;

        public ParameterBuilderCard(bool showDisplayName = true)
        {
            if (showDisplayName == true)
            {
                _displayNameLabel = new Label()
                    .Margin(0f, 0f, 0f, 4f)
                    .AddTo(this);
            }

            _root = new VisualElement()
                .Margin(2f)
                .Padding(4f)
                .BorderWidth(2f)
                .BorderRadius(8f)
                .BorderColor(new Color(0.37f, 0.37f, 0.37f))
                .AddTo(this);

            _title = new Label()
                .FontSize(12.5f)
                .FlexGrow(1f)
                .Bold()
                .AddTo(_root);

            SetupTitle();

            _expandRoot = new VisualElement().MarginTop(16f).AddTo(_root);
            _expandRoot.Display(DisplayStyle.None);
        }

        protected override VisualElement ExpandElementsContainer => _expandRoot;

        public ParameterBuilderCard Bind(SerializedProperty property)
        {
            _sourceBindProperty = property;
            _idProperty = property.FindPropertyRelative("_parameterId");
            _formula = property.FindPropertyRelative("_formula");

            if (_displayNameLabel != null)
                _displayNameLabel.text = property.displayName;

            _title.DisplayParameterName(_idProperty);
            return this;
        }
        
        protected override void Initialize()
        {
            BuildExpand(_expandRoot);
            BuildSettingsFields().AddTo(_expandRoot);

            BuildFormulaPart()
                .MarginTop(16f)
                .Padding(8f)
                .BorderRadius(8f)
                .AddTo(_expandRoot);

            _flatValueField.Bind(_sourceBindProperty.FindPropertyRelative("_flatValue"));
            _percentValueField.Bind(_sourceBindProperty.FindPropertyRelative("_percentValue"));

            _formulaControl.Bind(_idProperty, _formula);
        }

        private void SetupTitle()
        {
            _title.RegisterCallback<ClickEvent, ParameterBuilderCard>(
                (_, self) => self.SwitchExpandState(), this);

            _title.RegisterCallback<PointerEnterEvent, ParameterBuilderCard>(
                (_, self) => self._root.BackgroundColor(HoverColor), this);

            _title.RegisterCallback<PointerLeaveEvent, ParameterBuilderCard>(
                (_, self) => self._root.BackgroundColor(BackgroundColor), this);
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

        private VisualElement BuildSettingsFields()
        {
            var root = new VisualElement()
                .Margin(0f, 0f, 4f, 0f);

            _flatValueField = new FloatField("Flat:")
                .AddTo(root);

            _percentValueField = new FloatField("Percent:")
                .AddTo(root);

            return root;
        }

        private VisualElement BuildFormulaPart()
        {
            _formulaControl = new CalculationFormulaControl(true);
            
            _formulaControl.Label.ColorChangeAnimation(_formulaControl, new Color(0.25f, 0.25f, 0.25f),
                new Color(0.28f, 0.28f, 0.28f));

            return _formulaControl;
        }
    }
}