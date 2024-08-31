using System.Collections.Generic;
using System.Linq;
using Parameters.Runtime.Base;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [DefaultExecutionOrder(-1100)]
    public class ParameterInitializerMonoProvider : MonoBehaviour
    {
        [SerializeField] private ParameterData[] _parameters;

        private void Awake()
        {
            ParameterInitializer.Initialize(_parameters);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var assets = new List<ParameterData>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{nameof(ParameterData).ToLower()}");

            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ParameterData>(path);
                assets.Add(asset);
            }

            _parameters = assets.Where(x => x.Id != 0UL && x.Data != null).ToArray();
        }
#endif
    }
}