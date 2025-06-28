using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Parameters.Editor.Controls
{
    public class CalculationFormulaControl : VisualElement
    {
        private TextField _formulaInputField;
        private UsedParametersInFormulaExpandableList _usedParametersListView;
        private ReadOnlyExpandableList _formulaUsagesListView;
        private ReadOnlyExpandableList _dependenciesListView;
        private ReadOnlyExpandableList _descriptionsListView;

        private SerializedProperty _idProperty;

        private SerializedProperty _formula;
        private SerializedProperty _formulaUsedParameters;
        private SerializedProperty _formulaDependencies;
        private SerializedProperty _formulaUsages;
        private SerializedProperty _formulaDescription;

        private readonly VisualElement _expandRoot;

        public readonly Label Label;
        
        public CalculationFormulaControl(bool showPrepareButton)
        {
            Label = new Label("Formula")
                .FontSize(13f)
                .Bold()
                .BorderWidth(0f, 0f, 0f, 2f)
                .BorderColor(new Color(0.37f, 0.37f, 0.37f))
                .AddTo(this);

            Label.RegisterCallback<ClickEvent, CalculationFormulaControl>((_, self) =>
            {
                self._expandRoot.SwapDisplay();
            }, this);
            
            _expandRoot = new VisualElement()
                .Display(DisplayStyle.None)
                .AddTo(this);
            
            _formulaInputField = new TextField()
                .Margin(0f, 0f, 8f, 0f)
                .AddTo(_expandRoot);
            
            _formulaInputField.multiline = true;

            CreateUsedParametersListView(_expandRoot);
            
            CreateUsagesListView(_expandRoot);
            CreateDependenciesListView(_expandRoot);
            CreateDescriptionsListView(_expandRoot);

            var prepareButton = new Button()
                .SetText("PrepareFormula")
                .MarginTop(16f)
                .AddTo(_expandRoot);

            prepareButton.RegisterCallback<ClickEvent, CalculationFormulaControl>(
                (_, self) =>
                    EditorFormulaParser.Prepare(self._formula, self._idProperty.FindPropertyRelative("_id").intValue),
                this);
        }

        public CalculationFormulaControl Bind(SerializedProperty idProperty, SerializedProperty formulaProperty)
        {
            _idProperty = idProperty;
            _formula = formulaProperty;
            
            _formulaUsedParameters = _formula.FindPropertyRelative("_elements");
            _formulaDependencies = _formula.FindPropertyRelative("Dependencies");
            _formulaUsages = _formula.FindPropertyRelative("_usages");
            _formulaDescription = _formula.FindPropertyRelative("Descriptions");

            _formulaInputField.Bind(_formula.FindPropertyRelative("_formula"));

            _usedParametersListView.Bind(_formulaUsedParameters);
            _formulaUsagesListView.Bind(_formulaUsages);
            _dependenciesListView.Bind(_formulaDependencies);
            _descriptionsListView.Bind(_formulaDescription);
            
            return this;
        }
        
        private void CreateUsedParametersListView(VisualElement root)
        {
            _usedParametersListView = new UsedParametersInFormulaExpandableList("Used parameters")
                .MarginTop(4f)
                .AddTo(root);

            _usedParametersListView.AddButton.RegisterCallback<ClickEvent, CalculationFormulaControl>((_, self) =>
            {
                self._formulaUsedParameters.InsertArrayElementAtIndex(self._formulaUsedParameters.arraySize);
                var newItem = self._formulaUsedParameters.GetArrayElementAtIndex(self._formulaUsedParameters.arraySize - 1);
                newItem.FindPropertyRelative("_parameterIdProvider").FindPropertyRelative("_id").intValue = 0;
                newItem.FindPropertyRelative("_shortName").stringValue = string.Empty;
                self._formulaUsedParameters.serializedObject.ApplyModifiedProperties();
            }, this);
            
            _usedParametersListView.ListView.Editable();
            _usedParametersListView.ListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _usedParametersListView.ListView.makeItem = () =>
            {
                var parentElement = new VisualElement();

                var container = new VisualElement()
                    .MarginTop(16f)
                    .Padding(4f)
                    .AlignItems(Align.Center)
                    .FlexDirection(FlexDirection.Row)
                    .BorderRadius(8f)
                    .BackgroundColor(new Color(0.34f, 0.34f, 0.34f))
                    .AddTo(parentElement);
                
                new UsedFormulaParameterControl()
                    .FlexGrow(1f)
                    .Margin(0f, 4f, 0f, 0f)
                    .AddTo(container);

                EditorUIElementFactory
                    .CreateDeleteButton()
                    .AddTo(container);
                
                return parentElement;
            };
            _usedParametersListView.ListView.bindItem = (visualElement, index) =>
            {
                visualElement
                    .Q<UsedFormulaParameterControl>() 
                    .Bind(_formulaUsedParameters.GetArrayElementAtIndex(index));
                
                visualElement
                    .Q<Button>("delete-button")
                    .RegisterCallback<ClickEvent, (SerializedProperty prop, int i)>((_, tuple) =>
                    {
                        tuple.prop.DeleteArrayElementAtIndex(tuple.i);
                        tuple.prop.serializedObject.ApplyModifiedProperties();
                    }, (_formulaUsedParameters, index));
            };
        }

        private void CreateUsagesListView(VisualElement root)
        {
            _formulaUsagesListView = new ReadOnlyExpandableList("Usages")
                .MarginTop(16f)
                .AddTo(root);

            _formulaUsagesListView.ListView
                .FixedItemHeight(48f);

            _formulaUsagesListView.ListView.makeItem = () => new CalculationFormulaElementControl();
            _formulaUsagesListView.ListView.bindItem = (control, index) =>
                ((CalculationFormulaElementControl)control).Bind(_formulaUsages.GetArrayElementAtIndex(index));
        }

        private void CreateDependenciesListView(VisualElement root)
        {
            _dependenciesListView = new ReadOnlyExpandableList("Dependencies")
                .MarginTop(8f)
                .AddTo(root);

            _dependenciesListView.ListView.makeItem = () =>
            {
                var elRoot = new VisualElement();
                new Label()
                    .MarginTop(2f)
                    .Padding(4f)
                    .BackgroundColor(new Color(0.19f, 0.19f, 0.19f))
                    .AddTo(elRoot);

                return elRoot;
            };

            _dependenciesListView.ListView.bindItem = (label, index) =>
            {
                label.Q<Label>().text = DatabaseUtils.instance
                    .GetParameterName(_formulaDependencies.GetArrayElementAtIndex(index).intValue);
            };
        }

        private void CreateDescriptionsListView(VisualElement root)
        {
            _descriptionsListView = new ReadOnlyExpandableList("Descriptions")
                .MarginTop(8f)
                .AddTo(root);

            _descriptionsListView.ListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            _descriptionsListView.ListView.makeItem = () =>
            {
                var elRoot = new VisualElement();

                new FormulaElementDescriptionControl()
                    .MarginTop(16f)
                    .Padding(8f)
                    .BackgroundColor(new Color(0.27f, 0.27f, 0.27f))
                    .BorderRadius(8f)
                    .AddTo(elRoot);

                return elRoot;
            };

            _descriptionsListView.ListView.bindItem = (element, index) =>
            {
                element
                    .Q<VisualElement>()
                    .Q<FormulaElementDescriptionControl>().Bind(_formulaDescription.GetArrayElementAtIndex(index));
            };
        }
    }
}