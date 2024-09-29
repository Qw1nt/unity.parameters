using System.Collections.Generic;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using UnityEngine;

namespace Parameters.Runtime
{
    [CreateAssetMenu(menuName = "Parameters/DockerData")]
    public class ParametersDockerData : ScriptableObject
    {
        [SerializeField] private ParameterBuilder[] _parameters;

        public IReadOnlyList<ParameterBuilder> Parameters => _parameters;
        
        public ComplexParameterContainer Create(IParameterContainerHolder holder)
        {
            return new ComplexParameterContainer(holder, _parameters);
        }
    }
}