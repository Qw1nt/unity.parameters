using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [DefaultExecutionOrder(-100)]
    public class MonoDockerHolder : MonoBehaviour, IDockerHolder
    {
        [SerializeField] private ParametersDockerData.ParameterInDocker[] _parameters;
        
        public ParameterDocker Docker
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }
        
        private void Awake()
        {
            Docker = new ParameterDocker(this, _parameters);
        }
    }
}