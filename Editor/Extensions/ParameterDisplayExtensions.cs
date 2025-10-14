using Parameters.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;

namespace Parameters.Editor.Extensions
{
    public static class ParameterDisplayExtensions
    {
        public static void DisplayParameterName(this TextElement textElement, SerializedProperty idProperty)
        {
            var idValue = idProperty.FindPropertyRelative("_id");
            
            textElement.SetText(idValue.intValue == 0
                ? "Select parameter"
                : DatabaseUtils.instance.GetParameterName(idValue.intValue));
        }
    }
}