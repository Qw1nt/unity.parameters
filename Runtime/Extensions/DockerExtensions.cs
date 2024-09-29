using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Extensions
{
    public static class DockerExtensions 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexParameterContainer SafeGetDocker(this IParameterContainerHolder holder)
        {
            return ComplexParameterContainerStorage.GetSingle(holder.GetInstanceID());
        }   
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexParameterContainer GetDocker(this IParameterContainerHolder holder)
        {
            return ComplexParameterContainerStorage.GetSingle(holder.GetInstanceID());
        }

        public static void MakePlayerDocker(this ComplexParameterContainer container)
        {
            ComplexParameterContainerStorage.PlayerContainer = container;
        }
    }
}