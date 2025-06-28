using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Editor.Common;
using Parameters.Editor.Controls;
using Parameters.Editor.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Parameters.Editor.Windows
{
    public class ParameterSetupWindow : EditorWindow
    {
        private static readonly HashSet<Type> CachedTypes = new();

        private readonly List<ParameterSetupSerializeInfo> _sourceItems = new();
        private readonly List<ParameterSetupSerializeInfo> _listViewItems = new();

        private TextField _searchField;
        private ListView _listView;

        private void CreateGUI()
        {
            _searchField = new TextField("Search:")
                .MarginTop(4f);

            _searchField.isDelayed = true;
            _searchField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != evt.previousValue)
                    UpdateViewItems(evt.newValue);
            });

            _listView = new ListView(_listViewItems, ParameterSetupWindowListItem.ViewHeight, MakeListItem, BindItem)
            {
                showBorder = false,
                selectionType = SelectionType.None,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                itemsSource = _listViewItems
            };

            _listView.MarginTop(16f);

            var noHoverSheet = Resources.Load<StyleSheet>("Uss/ParametersListViewStyle");
            _listView.styleSheets.Add(noHoverSheet);

            rootVisualElement.Add(_searchField);
            rootVisualElement.Add(_listView);

            UpdateViewItems();
        }

        public void OnEnable()
        {
            if(DatabaseUtils.instance.IsReady == false)
                return;
 
            _sourceItems.Clear();
            _sourceItems.AddRange(DatabaseUtils.instance.GetInfos());
        }

        private void UpdateViewItems(string nameFilter = null)
        {
            _listViewItems.Clear();

            if (string.IsNullOrEmpty(nameFilter) == true)
            {
                _listViewItems.AddRange(_sourceItems);
            }
            else
            {
                foreach (var item in _sourceItems)
                {
                    var initializerName = item.GetInitializerName();
                    var friendlyName = item.FriendlyName.stringValue;

                    if (string.IsNullOrEmpty(friendlyName) == false)
                    {

                        if (IsValidString(friendlyName, nameFilter) == true)
                        {
                            _listViewItems.Add(item);
                            break;
                        }
                    }
                    
                    if (IsValidString(initializerName, nameFilter) == true)
                        _listViewItems.Add(item);
                }
            }

            _listView.Rebuild();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidString(string first, string second)
        {
            return first.IndexOf(second, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   first.IndexOf(second, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /*private bool TryLoadDatabase()
        {
            var databases = AssetDatabase.FindAssets($"t:{nameof(ParameterDatabase)}");

            switch (databases.Length)
            {
                case 0:
                    Debug.LogError("No ");
                    return false;

                case > 1:
                    Debug.LogError("More ");
                    return false;
            }

            var path = AssetDatabase.GUIDToAssetPath(databases[0]);
            _database = AssetDatabase.LoadAssetAtPath<ParameterDatabase>(path);
            _serializedDatabase = new SerializedObject(_database);

            return true;
        }*/

        private VisualElement MakeListItem()
        {
            return new ParameterSetupWindowListItem();
        }

        private void BindItem(VisualElement arg1, int arg2)
        {
            ((ParameterSetupWindowListItem)arg1).Init(_listViewItems[arg2]);
        }

        [MenuItem("Qw1nt/Parameters/Open Database")]
        private static void Open()
        {
            try
            {
                var window = GetWindow<ParameterSetupWindow>();
                window.titleContent = new GUIContent("Parameters setup Database");

                window.minSize = new Vector2(150f, 350f);
            }
            catch
            {
            }
        }
    }
}