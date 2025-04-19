using System.Collections.Generic;
using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Plugins.unity.parameters.Editor.Controls
{
    public class ParameterSetupWindowListItem : VisualElement
    {
        public const float ViewHeight = 64f;

        private static readonly Color SelectColor = new(0.45f, 0.45f, 0.45f);
        private static readonly Color DefaultColor = new(0.22f, 0.22f, 0.22f);

        private readonly VisualElement _innerContainer;
        private readonly List<VisualElement> _expandItems = new();

        private Label _nameLabel;
        private Label _namesDivider;
        private Label _friendlyNameLabel;

        private PropertyField _idPropertyField;
        private TextField _friendlyName;

        private bool _isOpen;

        public ParameterSetupWindowListItem() : base()
        {
            _innerContainer = new VisualElement()
            {
                style =
                {
                    marginBottom = 8f
                }
            };

            BuildCommonElements(_innerContainer);
            BuildExpandElement(_expandItems);

            style.height = StyleKeyword.Auto;

            foreach (var item in _expandItems)
                _innerContainer.Add(item);

            _innerContainer.Margin(4f)
                .Padding(8f)
                .BorderWidth(2f)
                .BorderRadius(8f)
                .BorderColor(SelectColor);

            Add(_innerContainer);
        }

        private void BuildCommonElements(VisualElement root)
        {
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;

            _nameLabel = new Label()
                .Bold()
                .FontSize(14f);

            container.RegisterCallback<ClickEvent, ParameterSetupWindowListItem>(
                (_, self) => self.ChangeShowState(!self._isOpen), this);

            container.RegisterCallback<PointerEnterEvent, ParameterSetupWindowListItem>(
                (_, self) => { self._innerContainer.BackgroundColor(SelectColor); }, this);

            container.RegisterCallback<PointerLeaveEvent, ParameterSetupWindowListItem>(
                (_, self) => { self._innerContainer.BackgroundColor(DefaultColor); }, this);

            _namesDivider = new Label("|")
                .Margin(4f, 4f, 0f, 0f)
                .FontSize(14f);

            _friendlyNameLabel = new Label()
                .Bold()
                .FontSize(14f);

            container.Add(_friendlyNameLabel);
            container.Add(_namesDivider);
            container.Add(_nameLabel);

            root.Add(container);
        }

        private void BuildExpandElement(List<VisualElement> items)
        {
            _idPropertyField = new PropertyField()
                .MarginTop(8f);

            _friendlyName = new TextField("Friendly Name:")
                .MarginTop(8f);

            _friendlyName.isDelayed = true;
            
            _friendlyName.RegisterValueChangedCallback(evt =>
            {
                if(evt.previousValue != evt.newValue)
                    UpdateNamesDivider(evt.newValue);
            });

            items.Add(_idPropertyField);
            items.Add(_friendlyName);
        }

        public void Init(ParameterSetupSerializeInfo info)
        {
            ChangeShowState(false);

            _friendlyNameLabel.BindProperty(info.FriendlyName);
            UpdateNamesDivider(info.FriendlyName.stringValue);
            _nameLabel.text = info.GetInitializerName();

            _idPropertyField.BindProperty(info.Id);
            _friendlyName.BindProperty(info.FriendlyName);
        }

        private void UpdateNamesDivider(string friendlyName)
        {
            _namesDivider.Display(string.IsNullOrEmpty(friendlyName) == true
                ? DisplayStyle.None
                : DisplayStyle.Flex);
        }

        private void ChangeShowState(bool isOpen)
        {
            var displayStyle = isOpen == true ? DisplayStyle.Flex : DisplayStyle.None;

            foreach (var item in _expandItems)
                item.Display(displayStyle);

            _isOpen = isOpen;
        }
    }
}