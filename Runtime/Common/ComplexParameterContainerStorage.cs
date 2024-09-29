using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Collections;

namespace Parameters.Runtime.Common
{
    internal static class ComplexParameterContainerStorage
    {
        internal static readonly Dictionary<int, SwapList<ComplexParameterContainer>> Map = new();
        internal static readonly SwapList<ComplexParameterContainer> Dockers = new();
        internal static ComplexParameterContainer PlayerContainer;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Add(ComplexParameterContainer container)
        {
            if (container.Holder == null)
                return;

            Dockers.Add(container);
            var holderId = container.Holder.GetInstanceID();

            if (Map.ContainsKey(holderId) == false)
                Map.Add(container.Holder.GetInstanceID(), new SwapList<ComplexParameterContainer>(2));
            
            Map[holderId].Add(container);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Remove(ComplexParameterContainer container)
        {
#if UNITY_EDITOR
            if (container.Holder == null)
                throw new NullReferenceException();
#endif

            Dockers.Remove(container);
            Map[container.Holder.GetInstanceID()].Remove(container);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static SwapList<ComplexParameterContainer> Get(int id)
        {
            return Map[id];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ComplexParameterContainer GetSingle(int id)
        {
            return Map[id].Items[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ComplexParameterContainer SafeGetFirst(int id)
        {
            if (Map.ContainsKey(id) == false)
                return null;

            var list = Map[id];
            return list.Items[0];
        }
    }
}