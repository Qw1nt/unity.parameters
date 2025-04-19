using UnityEngine.UIElements;

namespace Parameters.Editor.Extensions
{
    public static class TextElementExtensions
    {
        public static T SetText<T>(this T element, string text)
            where T : TextElement
        {
            element.text = text;
            return element;
        }
    }
}