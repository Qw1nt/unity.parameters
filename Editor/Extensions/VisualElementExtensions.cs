using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Extensions
{
    public static class VisualElementExtensions
    {
        public static T FontSize<T>(this T element, float value)
            where T : VisualElement
        {
            element.style.fontSize = value;
            return element;
        }      
        
        public static T Bold<T>(this T element)
            where T : VisualElement
        {
            element.style.unityFontStyleAndWeight = FontStyle.Bold;
            return element;
        }
        
        public static T Display<T>(this T element, DisplayStyle style)
            where T : VisualElement
        {
            element.style.display = style;
            return element;
        }

        public static T Padding<T>(this T element, float value)
            where T : VisualElement
        {
            element.style.paddingBottom = value;
            element.style.paddingLeft = value;
            element.style.paddingRight = value;
            element.style.paddingTop = value;

            return element;
        }

        public static T Margin<T>(this T element, float value)
            where T : VisualElement
        {
            element.style.marginLeft = value;
            element.style.marginRight = value;
            element.style.marginTop = value;
            element.style.marginBottom = value;

            return element;
        }
        
        public static T Margin<T>(this T element, float left, float right, float top, float bottom)
            where T : VisualElement
        {
            element.style.marginLeft = left;
            element.style.marginRight = right;
            element.style.marginTop = top;
            element.style.marginBottom = bottom;

            return element;
        }
        
        public static T MarginTop<T>(this T element, float value)
            where T : VisualElement
        {
            element.style.marginTop = value;
            return element;
        }

        public static T BorderWidth<T>(this T element, float width)
            where T : VisualElement
        {
            element.style.borderBottomWidth = width;
            element.style.borderLeftWidth = width;
            element.style.borderRightWidth = width;
            element.style.borderTopWidth = width;

            return element;
        }

        public static T BorderRadius<T>(this T element, float value)
            where T : VisualElement
        {
            element.style.borderBottomLeftRadius = value;
            element.style.borderBottomRightRadius = value;
            element.style.borderTopLeftRadius = value;
            element.style.borderTopRightRadius = value;

            return element;
        }

        public static T BorderColor<T>(this T element, Color value)
            where T : VisualElement
        {
            element.style.borderTopColor = value;
            element.style.borderBottomColor = value;
            element.style.borderLeftColor = value;
            element.style.borderRightColor = value;

            return element;
        }
        
        public static T BackgroundColor<T>(this T element, Color value)
            where T : VisualElement
        {
            element.style.backgroundColor = value;
            return element;
        }
    }
}