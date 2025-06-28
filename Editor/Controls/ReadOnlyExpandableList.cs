using Parameters.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Controls
{
    public class ReadOnlyExpandableList : VisualElement
    {
        protected readonly Color BackgroundColor = new(0.19f, 0.19f, 0.19f);
        protected EventCallback<ClickEvent, ReadOnlyExpandableList> ToggleCallback;
        
        public ReadOnlyExpandableList(string labelContent)
        {
            Label = new Label(labelContent)
                .FontSize(11f)
                .Bold()
                .AddTo(this);
            
            ListView = new ListView()
                .HideSizeCounter()
                .ReadOnly()
                .AddTo(this);

            this.BackgroundColor(BackgroundColor)
                .Padding(8f)
                .BorderRadius(8f);

            ToggleCallback = (_, self) => self.ChangeExpandState();
            RegisterCallback(ToggleCallback, this);
            
            ListView.Display(DisplayStyle.None);
        }
        
        public Label Label { get; private set; }
        
        public ListView ListView { get; private set; }

        public ReadOnlyExpandableList Bind(SerializedProperty listViewBindProperty)
        {
            ListView.Bind(listViewBindProperty);
            return this;
        }
        
        protected virtual void ChangeExpandState()
        {
            ListView.SwapDisplay();
        }
    }
}