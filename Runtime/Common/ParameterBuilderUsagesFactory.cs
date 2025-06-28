#if UNITY_EDITOR
using System.Collections.Generic;
using Parameters.Runtime.Base;
using Parameters.Runtime.CalculationFormulas;
using UnityEditor;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    public class ParameterBuilderUsagesFactory : ScriptableSingleton<ParameterBuilderUsagesFactory>
    {
        private const string ParameterIdProvider = "_parameterIdProvider";
        private const string ShortName = "_shortName";
        
        [SerializeField] private List<UsedFormulaParameter> _elements;
        
        public List<UsedFormulaParameter> Build(ParameterData1 required, List<UsedFormulaParameter> other)
        {
            var serializedObject = new SerializedObject(this);
            var array = serializedObject.FindProperty(nameof(_elements));

            array.arraySize = other.Count + 1;
            var requiredSerialized = array.GetArrayElementAtIndex(0);
            SetupElement(requiredSerialized, required, "value");

            for (int i = 1; i < array.arraySize; i++)
                SetupElement(array.GetArrayElementAtIndex(i), other[i - 1].ParameterData, other[i - 1].ShortName);

            serializedObject.ApplyModifiedProperties();
            
            return _elements;
        }        
        
        public void Build(int requiredId, SerializedProperty elements, SerializedProperty usages)
        {
            for (int i = 0; i < elements.arraySize; i++)
            {
                var element = elements.GetArrayElementAtIndex(i);
                var elementId = element.FindPropertyRelative(ParameterIdProvider).FindPropertyRelative("_id").intValue;
                var elementShortName = element.FindPropertyRelative(ShortName).stringValue;
                
                usages.InsertArrayElementAtIndex(0);
                var addedElement = usages.GetArrayElementAtIndex(0);

                addedElement.FindPropertyRelative(ParameterIdProvider).FindPropertyRelative("_id").intValue = elementId;
                addedElement.FindPropertyRelative(ShortName).stringValue = elementShortName;
            }

            usages.InsertArrayElementAtIndex(0);
            var currentElement = usages.GetArrayElementAtIndex(0);

            currentElement.FindPropertyRelative(ParameterIdProvider).FindPropertyRelative("_id").intValue = requiredId;
            currentElement.FindPropertyRelative(ShortName).stringValue = "value";
            
            return;
            /*var serializedObject = new SerializedObject(this);
            var array = serializedObject.FindProperty(nameof(_elements));

            array.arraySize = elements.arraySize + 1;
            var requiredSerialized = array.GetArrayElementAtIndex(0);
            
            SetupElement(requiredSerialized, requiredIdProvider, "value");

            for (int i = 1; i < array.arraySize; i++)
                SetupElement(array.GetArrayElementAtIndex(i), other[i - 1].ParameterData, other[i - 1].ShortName);

            serializedObject.ApplyModifiedProperties();*/
            
        }

        private void SetupElement(SerializedProperty element, ParameterData1 reference, string shortName)
        {
            element.FindPropertyRelative("_parameter").objectReferenceValue = reference;
            element.FindPropertyRelative("_shortName").stringValue = shortName;

        }
    }
}
#endif