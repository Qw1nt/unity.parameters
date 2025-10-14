using UnityEditor;

namespace Parameters.Editor.Extensions
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty GetIdProperty(this SerializedProperty formulaElement)
        {
            return formulaElement.FindPropertyRelative("_parameterIdProvider").FindPropertyRelative("_id");
        }
    }
}