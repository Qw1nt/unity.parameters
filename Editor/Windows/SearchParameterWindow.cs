using System;
using System.Collections.Generic;
using Parameters.Editor.Common;
using Parameters.Editor.Extensions;
using Parameters.Runtime.Base;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Parameters.Editor.Windows
{
    public class SearchParameterWindow : ScriptableObject, ISearchWindowProvider
    {
        private Action<int> _onSelectCallback;

        public void SetSelectCallback(Action<int> onSelectCallback)
        {
            _onSelectCallback = onSelectCallback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var validInfos = new List<ParameterSetupSerializeInfo>(DatabaseUtils.instance.GetInfos());
            validInfos.RemoveAll(x => x.IdHash.intValue == 0);
            
            var result = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Parameters"), 0) };
            
            result.Add(new SearchTreeGroupEntry(new GUIContent("Default names"), 1));

            foreach (var info in validInfos)
            {
                result.Add(new SearchTreeEntry(new GUIContent(info.GetInitializerName()))
                {
                    userData = info.IdHash.intValue,
                    level = 2
                });
            } 
            
            result.Add(new SearchTreeGroupEntry(new GUIContent("Friendly names"), 1));

            foreach (var info in validInfos)
            {
                var friendlyName = info.FriendlyName.stringValue;

                if (string.IsNullOrEmpty(friendlyName) == true)
                    continue;
                
                result.Add(new SearchTreeEntry(new GUIContent(friendlyName))
                {
                    userData = info.IdHash.intValue,
                    level = 2
                });
            }
            
            return result;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _onSelectCallback?.Invoke((int) searchTreeEntry.userData);
            return true;
        }
    }
}