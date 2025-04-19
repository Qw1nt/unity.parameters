using Parameters.Editor.Extensions;
using UnityEditor;

namespace Parameters.Editor.Common
{
    public class ParameterSetupSerializeInfo
    {
        public ParameterSetupSerializeInfo(SerializedProperty item)
        {
            Id = item.FindPropertyRelative("_id");
            Initializer = item.FindPropertyRelative("Initializer");
            FriendlyName = item.FindPropertyRelative("_friendlyName");
        }
        
        public SerializedProperty Id { get; }
        
        public SerializedProperty Initializer { get; }
        
        public SerializedProperty FriendlyName { get; }

        public string GetInitializerName()
        {
            return Initializer.managedReferenceValue.GetType().GetDisplayName();
        }
    }
}