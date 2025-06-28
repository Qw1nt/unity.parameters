using Parameters.Editor.Extensions;
using Parameters.Runtime.CalculationFormulas;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.Controls
{
    /// <summary>
    /// <see cref="FormulaElementDescription"/>
    /// </summary>
    public class FormulaElementDescriptionControl : VisualElement
    {
        private const float LabelWidth = 64f;
        
        private readonly UnsignedLongField _hashField;

        private readonly ElementGroup<EnumField> _sources;
        private readonly ElementGroup<IntegerField> _indexes;
        private readonly ElementGroup<IntegerField> _parametersIds;
        private readonly ElementGroup<FloatField> _simpleValues;
        
        private readonly EnumField _operationType;
        
        public FormulaElementDescriptionControl()
        {
            _hashField = new UnsignedLongField("Hash")
                .AddTo(this);
            
            _sources = new ElementGroup<EnumField>("Source:")
                .MarginTop(4f)
                .AddTo(this);

            _indexes = new ElementGroup<IntegerField>("Indexes:")
                .MarginTop(4f)
                .AddTo(this);
            
            _parametersIds = new ElementGroup<IntegerField>("Ids:")
                .MarginTop(4f)
                .AddTo(this);
            
            _simpleValues = new ElementGroup<FloatField>("Simple:")
                .MarginTop(4f)
                .AddTo(this);
            
            _operationType = new EnumField("Operation type: ")
                .MarginTop(8f)
                .AddTo(this);
        }

        public void Bind(SerializedProperty property)
        {
            _hashField.BindProperty(property.FindPropertyRelative(nameof(FormulaElementDescription.Hash)));
            
            _sources
                .SetProperty(property)
                .BindLeft(nameof(FormulaElementDescription.LeftSource))
                .BindRight(nameof(FormulaElementDescription.RightSource));

            _indexes
                .SetProperty(property)
                .BindLeft(nameof(FormulaElementDescription.LeftIndex))
                .BindRight(nameof(FormulaElementDescription.RightIndex));
         
            _parametersIds
                .SetProperty(property)
                .BindLeft(nameof(FormulaElementDescription.LeftParameterId))
                .BindRight(nameof(FormulaElementDescription.RightParameterId));
       
            _simpleValues
                .SetProperty(property)
                .BindLeft(nameof(FormulaElementDescription.SimpleLeft))
                .BindRight(nameof(FormulaElementDescription.SimpleLeft));
            
            _operationType.Bind(property.FindPropertyRelative("OperationType"));
        }
        
        private class ElementGroup<TItem> : VisualElement
            where TItem : VisualElement, IBindable, new()
        {
            private SerializedProperty _property;

            public readonly TItem Left;
            public readonly TItem Right;
            
            public ElementGroup(string label)
            {
                new Label(label)
                    .Bold()
                    .FontSize(12f)
                    .Width(LabelWidth)
                    .AddTo(this);

                Left = new TItem()
                    .FlexGrow(1f)
                    .Margin(0f, 4f, 0f, 0f)
                    .AddTo(this);

                Right = new TItem()
                    .FlexGrow(1f)
                    .AddTo(this);
                
                this.AlignItems(Align.Center)
                    .FlexDirection(FlexDirection.Row);
            }

            public ElementGroup<TItem> SetProperty(SerializedProperty property)
            {
                _property = property;
                return this;
            }
            
            public ElementGroup<TItem> BindLeft(string propertyName)
            {
                Left.Bind(_property.FindPropertyRelative(propertyName));
                return this;
            }      
            
            public ElementGroup<TItem> BindRight(string nameProperty)
            {
                Right.Bind(_property.FindPropertyRelative(nameProperty));
                return this;
            }
        }
    }
}