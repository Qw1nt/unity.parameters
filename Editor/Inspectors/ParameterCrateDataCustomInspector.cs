using System;
using Parameters.Editor.Extensions;
using Parameters.Editor.Windows;
using Parameters.Runtime.Base;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Parameters.Editor.Inspectors
{
    [CustomEditor(typeof(ParameterData1))]
    public class ParameterCrateDataCustomInspector : UnityEditor.Editor
    {
        private string _selectedTypeName;
        private ParameterData1 _data;

        private void OnEnable()
        {
            _data = (ParameterData1)target;
            TrySetSelectedTypeName();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Initializator: ");

            var buttonText = string.IsNullOrEmpty(_selectedTypeName) == true
                ? "Select Type"
                : _selectedTypeName;

            if (GUILayout.Button(buttonText, EditorStyles.popup) == true)
                SetReference();

            EditorGUILayout.EndHorizontal();
        }

        private void SetReference()
        {
            var window = CreateInstance<SearchParameterTypeWindow>();
            window.SetSelectCallback(type =>
            {
                var instance = Activator.CreateInstance(type);
                _data.Data = instance;
                
                TrySetSelectedTypeName();
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(target);
            });

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), window);
        }

        private void TrySetSelectedTypeName()
        {
            if (_data.Data != null)
                _selectedTypeName = _data.Data.GetType().GetDisplayName();
        }
    }
}