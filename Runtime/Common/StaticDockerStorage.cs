using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    internal static class StaticDockerStorage
    {
        internal static readonly Dictionary<int, FastList<ParameterDocker>> Map = new();
        internal static readonly FastList<ParameterDocker> Dockers = new();
        internal static ParameterDocker PlayerDocker;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Add(ParameterDocker docker)
        {
            if (docker.Holder == null)
                return;

            Dockers.Add(docker);
            var holderId = docker.Holder.GetInstanceID();

            if (Map.ContainsKey(holderId) == false)
                Map.Add(docker.Holder.GetInstanceID(), new FastList<ParameterDocker>(2));
            
            Map[holderId].Add(docker);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Remove(ParameterDocker docker)
        {
#if UNITY_EDITOR
            if (docker.Holder == null)
                throw new NullReferenceException();
#endif

            Dockers.Remove(docker);
            Map[docker.Holder.GetInstanceID()].Remove(docker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FastList<ParameterDocker> Get(int id)
        {
            return Map[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ParameterDocker GetSingle(int id)
        {
            return Map[id].data[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ParameterDocker SafeGetFirst(int id)
        {
            if (Map.ContainsKey(id) == false)
                return null;

            var list = Map[id];
            return list.data[0];
        }
    }
}