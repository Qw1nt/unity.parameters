using System;
using System.Collections.Generic;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

#if PARAMETERS_TRI_INSPECTOR
using TriInspector;
#endif

namespace Parameters.Runtime
{
    [CreateAssetMenu(menuName = "Parameters/DockerData")]
    public class ParametersDockerData : ScriptableObject
    {
        [SerializeField] private ParameterCrateInDocker[] _parameters;

        public IReadOnlyList<ParameterCrateInDocker> Parameters => _parameters;

        public ParameterDocker Create(IDockerHolder holder)
        {
            return new ParameterDocker(holder, _parameters);
        }

        [Serializable]
        public class ParameterCrateInDocker : IParameterCrateFactory
        {
#if PARAMETERS_TRI_INSPECTOR
            [Title("$" + nameof(CrateName))]
#endif
            [FormerlySerializedAs("_crateCrate")] [SerializeField] private ParameterCrateData _crate;

            [Space] [SerializeField] private bool _withDefaultValue;
            [SerializeField] private float _defaultValue;

            private string CrateName => _crate.DebugName;

            public ulong Id => _crate.Id;

            public float DefaultValue => _defaultValue;

            public IParameterCrate CreateCrate(ParameterDocker docker)
            {
                var instance = _crate.CreateCrate(docker);

                if (_withDefaultValue == true)
                    _crate.AddDefaultValue(instance, _defaultValue);

                return instance;
            }
        }
    }
}