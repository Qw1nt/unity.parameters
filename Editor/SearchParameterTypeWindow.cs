using System;
using System.Collections.Generic;
using Parameters.Runtime.Base;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Parameters.Editor
{
    public class SearchParameterTypeWindow : ScriptableObject, ISearchWindowProvider
    {
        private ParameterData _target;
        private Action<Type> _onSelectCallback;

        public void SetSelectCallback(Action<Type> onSelectCallback)
        {
            _onSelectCallback = onSelectCallback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var result = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Types"), 0) };
            var parametersMap = EditorParametersStorage.Data;

            foreach (var record in parametersMap)
            {
                result.Add(new SearchTreeEntry(new GUIContent(record.Value))
                {
                    level = 1,
                    userData = record.Key
                });
            }

            return result;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _onSelectCallback?.Invoke((Type) searchTreeEntry.userData);
            return true;
        }
    }
}