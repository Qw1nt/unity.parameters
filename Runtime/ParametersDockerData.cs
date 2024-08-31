using System;
using System.Collections.Generic;
using Parameters.Runtime.Base;
using Parameters.Runtime.CalculationFormulas;
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
        [SerializeField] private ParameterInDocker[] _parameters;

        public IReadOnlyList<ParameterInDocker> Parameters => _parameters;

        public ParameterDocker Create(IDockerHolder holder)
        {
            return new ParameterDocker(holder, _parameters);
        }

        [Serializable]
        public class ParameterInDocker : IParameterFactory
        {
#if PARAMETERS_TRI_INSPECTOR
            [Title("$" + nameof(CrateName))]
#endif
            [FormerlySerializedAs("_crateCrate")] [SerializeField] private ParameterData _crate;

            [Space] [SerializeField] private bool _withDefaultValue;
            [SerializeField] private float _defaultValue;
            [SerializeField] private float _overallValue = 1f;

            [Space] [SerializeField] private CalculationFormula _formula;
            
            private string CrateName => _crate?.DebugName;

            public ulong Id => _crate.Id;

            public float DefaultValue => _defaultValue;

            public FormulaElementDescription[] Formula => _formula.Descriptions;

            public Parameter CreateParameter(ParameterDocker docker)
            {
                var instance = _crate.CreateParameter(docker);

                if (_withDefaultValue == true)
                    instance.Value.CleanValue = _defaultValue;

                instance.Overall.CleanValue = _overallValue;
                
                return instance;
            }
        }
    }
}