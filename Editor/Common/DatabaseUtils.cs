using System;
using System.Collections.Generic;
using System.Linq;
using Parameters.Editor.Extensions;
using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Parameters.Editor.Common
{
    internal class DatabaseUtils : ScriptableSingleton<DatabaseUtils>
    {
        private readonly HashSet<Type> _processedInitializers = new();
        private readonly List<ParameterSetupSerializeInfo> _serializeSetupInfos = new();
        private readonly Dictionary<int, ParameterSetupSerializeInfo> _serializeSetupInfoMap = new();

        private SerializedProperty _databaseValues;

        public ParameterDatabase Database { get; private set; }

        public SerializedObject SerializedDatabase { get; private set; }

        public bool IsReady { get; private set; }

        [DidReloadScripts]
        private static void ReloadDatabase()
        {
            var databases = AssetDatabase.FindAssets($"t:{nameof(ParameterDatabase)}");

            switch (databases.Length)
            {
                case 0:
                    Debug.LogError("No ");
                    instance.IsReady = false;
                    break;

                case > 1:
                    Debug.LogError("More ");
                    instance.IsReady = false;
                    break;
            }

            var path = AssetDatabase.GUIDToAssetPath(databases[0]);

            instance.Database = AssetDatabase.LoadAssetAtPath<ParameterDatabase>(path);
            instance.SerializedDatabase = new SerializedObject(instance.Database);
            instance._databaseValues = instance.SerializedDatabase.FindProperty("_values");
            instance.UpdateDatabaseValues();

            instance.IsReady = true;
        }

        private void UpdateDatabaseValues()
        { 
            var allInitializers = TypeCache.GetTypesWithAttribute<ParameterInitSelfAttribute>();
            var alphabetSorted = allInitializers.OrderBy(x => x.Name).ToList();

            _processedInitializers.Clear();
            _serializeSetupInfos.Clear();
            _serializeSetupInfoMap.Clear();

            for (int i = 0; i < _databaseValues.arraySize; i++)
            {
                var arrayElement = _databaseValues.GetArrayElementAtIndex(i);
                var initializer = arrayElement.FindPropertyRelative("Initializer").managedReferenceValue;

                if (initializer == null)
                {
                    _databaseValues.DeleteArrayElementAtIndex(i);
                    i--;
                    continue;
                } 

                var initializerType = initializer.GetType();

                if (_processedInitializers.Contains(initializerType) == true)
                    continue;

                _processedInitializers.Add(initializerType);
            }

            for (int i = 0; i < alphabetSorted.Count; i++)
            {
                var initializerType = alphabetSorted[i];

                if (_processedInitializers.Contains(initializerType) == true)
                    continue;

                _databaseValues.InsertArrayElementAtIndex(i);
                var element = _databaseValues.GetArrayElementAtIndex(i);

                var initializer = element.FindPropertyRelative("Initializer");
                initializer.managedReferenceValue = Activator.CreateInstance(initializerType);

                _processedInitializers.Add(initializerType);
            }

            for (int i = 0; i < _databaseValues.arraySize; i++)
            {
                var obj = new ParameterSetupSerializeInfo(_databaseValues.GetArrayElementAtIndex(i));
                _serializeSetupInfos.Add(obj);

                var idHash = obj.Id.FindPropertyRelative("_hash");
                
                if (idHash.intValue != 0)
                    _serializeSetupInfoMap.Add(idHash.intValue, obj);
            }

            instance.SerializedDatabase.ApplyModifiedProperties();
        }

        public IReadOnlyList<ParameterSetupSerializeInfo> GetInfos()
        {
            return _serializeSetupInfos;
        }

        public IReadOnlyList<ParameterSetupSerializeInfo> SetupInfo()
        {
            return null;
        }

        public ParameterSetupSerializeInfo GetInfo(int id)
        {
            return id == 0 
                ? null 
                : _serializeSetupInfoMap[id];
        }
        
        public string GetParameterName(int id)
        {
            var info = GetInfo(id);

            if (info == null)
                return string.Empty;
            
            var friendlyName = info.FriendlyName.stringValue;

            return string.IsNullOrEmpty(friendlyName) == false
                ? friendlyName
                : info.GetInitializerName();
        }
    }
}