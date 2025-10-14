using Parameters.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Controls
{
    public class UsedParametersInFormulaExpandableList : ReadOnlyExpandableList
    {
        private readonly Color _backgroundColor;
        private readonly Color _highlightColor = new(0.22f, 0.22f, 0.22f);

        private readonly VisualElement _listViewContainer;

        public readonly Button AddButton;
        
        public UsedParametersInFormulaExpandableList(string labelContent) : base(labelContent)
        {
            _backgroundColor = BackgroundColor;

            UnregisterCallback(ToggleCallback);
            Label.RegisterCallback(ToggleCallback, this);

            Label.RegisterCallback<PointerEnterEvent, UsedParametersInFormulaExpandableList>(
                (_, self) => { self.BackgroundColor(self._highlightColor); }, this);

            Label.RegisterCallback<PointerLeaveEvent, UsedParametersInFormulaExpandableList>(
                (_, self) => { self.BackgroundColor(self._backgroundColor); }, this);

            _listViewContainer = new VisualElement()
                .AddTo(this);

            Remove(ListView);

            ListView
                .Display(DisplayStyle.Flex)
                .SelectionType(SelectionType.None)
                .AddTo(_listViewContainer);

            AddButton = new Button()
                .MarginTop(8f)
                .SetText("+")
                .Bold()
                .FontSize(13f)
                .BackgroundColor(new Color(0.41f, 0.41f, 0.54f))
                .AddTo(_listViewContainer);

            _listViewContainer.Display(DisplayStyle.None);
        }

        protected override void ChangeExpandState()
        {
            _listViewContainer.SwapDisplay();
        }
    }
}