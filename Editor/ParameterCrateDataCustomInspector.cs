using System;
using System.Reflection;
using Parameters.Runtime.Base;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Parameters.Editor
{
    [CustomEditor(typeof(ParameterData))]
    public class ParameterCrateDataCustomInspector : UnityEditor.Editor
    {
        private string _selectedTypeName;
        private ParameterData _data;

        private void OnEnable()
        {
            _data = (ParameterData)target;
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
                _data.Data = instance;

                TrySetSelectedTypeName();
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(_data);
            });

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), window);
        }

        private void TrySetSelectedTypeName()
        {
            if (_data.Data != null)
                _selectedTypeName = _data.Data.GetType().Name.Split('.')[^1];
        }
    }
}