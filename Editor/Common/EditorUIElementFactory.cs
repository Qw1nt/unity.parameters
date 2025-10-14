using Parameters.Editor.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Common
{
    public static class EditorUIElementFactory
    {
        public static Button CreateDeleteButton()
        {
            return new Button()
                .SetName("delete-button")
                .SetText("\u00d7")
                .Bold()
                .FontSize(13f)
                .Height(24f)
                .Width(24f)
                .BackgroundColor(new Color(1f, 0.42f, 0.38f));
        }
    }
}