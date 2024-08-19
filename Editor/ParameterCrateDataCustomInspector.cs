using System;
using System.Reflection;
using Parameters.Runtime.Base;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Parameters.Editor
{
    [CustomEditor(typeof(ParameterCrateData))]
    public class ParameterCrateDataCustomInspector : UnityEditor.Editor
    {
        private string _selectedTypeName;
        private ParameterCrateData _crateData;

        private void OnEnable()
        {
            _crateData = (ParameterCrateData)target;
            TrySetSelectedTypeName();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Class: ");

            var buttonText = string.IsNullOrEmpty(_selectedTypeName) == true
                ? "Select Type"
                : _selectedTypeName;

            if (GUILayout.Button(buttonText, EditorStyles.popup) == true)
                BuildMenu();

            EditorGUILayout.EndHorizontal();
        }

        private void BuildMenu()
        {
            var window = CreateInstance<SearchParameterTypeWindow>();
            window.SetSelectCallback(type =>
            {
                var constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.NonPublic, 
                    null, 
                new Type[] {}, 
                    null);
                
                var instance = constructor!.Invoke(new object[] {}); //Activator.CreateInstance(type);
                _crateData.Data = instance;

                TrySetSelectedTypeName();
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(_crateData);
            });

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), window);
        }

        private void TrySetSelectedTypeName()
        {
            if (_crateData.Data != null)
                _selectedTypeName = _crateData.Data.GetType().Name.Split('.')[^1];
        }
    }
}