using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Parameters.Runtime.Common
{
    public class MonoParameterContainerHolder : MonoBehaviour, IParameterContainerHolder
    {
        [FormerlySerializedAs("_docker")] [SerializeField] private ComplexParameterContainerBuilder _complexParameterContainer;

        private ComplexParameterContainer _instance;
        
        public ComplexParameterContainer Container
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = _complexParameterContainer.Create(this);
                return _instance;
            }
        }
    }
}