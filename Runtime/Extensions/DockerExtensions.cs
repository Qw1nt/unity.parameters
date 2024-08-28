using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Extensions
{
    public static class DockerExtensions 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterDocker SafeGetDocker(this IDockerHolder holder)
        {
            return StaticDockerStorage.GetSingle(holder.GetInstanceID());
        }   
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterDocker GetDocker(this IDockerHolder holder)
        {
            return StaticDockerStorage.GetSingle(holder.GetInstanceID());
        }

        public static void MakePlayerDocker(this ParameterDocker docker)
        {
            StaticDockerStorage.PlayerDocker = docker;
        }
    }
}