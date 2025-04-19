using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;
using SaintsField.Playa;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public class ComplexParameterContainerBuilder
    {
        [SerializeField] private TestEditor _te;
        [SerializeField] private ParameterBuilder[] _parameters;

#if UNITY_EDITOR
        [Button]
        private void Prepare()
        {
            foreach (var parameter in _parameters)
                parameter.PrepareFormula();
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameterContainer Create(IParameterContainerHolder holder, ComplexParameterContainer parent = null)
        {
            return new ComplexParameterContainer(holder, _parameters, parent);
        }
    }
}