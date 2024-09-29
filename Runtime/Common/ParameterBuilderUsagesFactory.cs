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
        [SerializeField] private List<CalculationFormulaElement> _elements;
        
        public List<CalculationFormulaElement> Build(ParameterData required, List<CalculationFormulaElement> other)
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

        private void SetupElement(SerializedProperty element, ParameterData reference, string shortName)
        {
            element.FindPropertyRelative("_parameter").objectReferenceValue = reference;
            element.FindPropertyRelative("_shortName").stringValue = shortName;

        }
    }
}
#endif