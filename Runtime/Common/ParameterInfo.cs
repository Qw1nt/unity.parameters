using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [Serializable]
    public struct ParameterInfo : IParameterStaticIdSetter
    {
        [SerializeField] private Id _id;

#if UNITY_EDITOR
        [SerializeField] private string _friendlyName;
#endif

        [SerializeReference] public IParameterStaticIdSetter Initializer;

        public int Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _id;
        }

        public void SetStaticId(int id)
        {
            Initializer.SetStaticId(_id);
        }
    }
}