using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Extensions
{
    public static class HoverAnimationExtensions
    {
        public static TElement ColorChangeAnimation<TElement>(this TElement element, VisualElement target,
            Color defaultColor, Color highlight)
            where TElement : VisualElement
        {
            var data = new ColorChangeData
            {
                Target = target,
                Default = defaultColor,
                Highlight = highlight
            };
            
            element.RegisterCallback<PointerEnterEvent, ColorChangeData>((_, self) =>
            {
                self.Target.BackgroundColor(self.Highlight);
            }, data);

            element.RegisterCallback<PointerLeaveEvent, ColorChangeData>((_, self) =>
            {
                self.Target.BackgroundColor(self.Default);
            }, data);
            
            data.Target.BackgroundColor(data.Default);
            
            return element;
        }
        
        private struct ColorChangeData
        {
            public VisualElement Target;
            
            public Color Default;
            public Color Highlight;
        }
    }
}