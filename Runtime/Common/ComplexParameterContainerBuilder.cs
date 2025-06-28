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
        [SerializeField] private ParameterBuilder[] _parameters;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameterContainer Create(IParameterContainerHolder holder, ComplexParameterContainer parent = null)
        {
            return new ComplexParameterContainer(holder, _parameters, parent);
        }
    }
}