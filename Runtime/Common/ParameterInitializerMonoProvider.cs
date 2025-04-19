using System;
using UnityEditor;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [DefaultExecutionOrder(-1100)]
    public class ParameterInitializerMonoProvider : MonoBehaviour
    {
        [SerializeField] private ParameterDatabase _database;

        private void Awake()
        {
            ParameterInitializer.Initialize(_database.GetValidRecordsWithAlloc());
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_database != null)
                return;

            var guids = AssetDatabase.FindAssets($"t:{nameof(ParameterDatabase)}");

            if (guids.Length > 1 || guids.Length == 0)
                throw new Exception();

            _database = AssetDatabase.LoadAssetAtPath<ParameterDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
#endif
    }
}