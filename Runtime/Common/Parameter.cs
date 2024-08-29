using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    public class Parameter : IReadOnlyParameter
    {
        public readonly ulong Id;
        
        private bool _hasChanges;
        
        internal readonly FastList<ParameterRawValue> Values = new(1);
        internal readonly FastList<ParameterRawValue> Overalls = new(1);
        internal readonly ParameterDocker Docker;

        internal ParameterRawValue Value;
        internal ParameterRawValue Overall;

        internal FastList<CrateUpdateSubscriberBase> Subscribers;

        internal Parameter(ulong id, ParameterDocker docker)
        {
            Id = id;
            Docker = docker;
        }

        public virtual void SetStaticId(ulong id)
        {
        }

        public virtual Parameter CreateInstance(float rawValue = 0f, float rawOverallValue = 1f)
        {
#if UNITY_EDITOR
            throw new Exception();
#endif

            return null;
        }

        public virtual Parameter CreateInstance(Id id, CrateType type, ParameterDocker docker)
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanRawValue()
        {
            return Value.CleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetRawValue()
        {
            return Value.ParentModifiedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanRawOverallValue()
        {
            return Overall.CleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetRawOverallValue()
        {
            return Overall.ParentModifiedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(float rawValue)
        {
            Value.CleanValue += rawValue;
            Docker.CalculationBuffer.Add(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(ref ParameterRawValue value)
        {
            for (int i = 0; i < Values.length; i++)
            {
                if(Values.data[i].Hash != value.Hash)
                    continue;

                Values.data[i].CleanValue = value.CleanValue;
                Docker.CalculationBuffer.Add(Id);
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subtract(float rawValue)
        {
            Value.CleanValue -= rawValue;
            Docker.CalculationBuffer.Add(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOverall(float rawValue)
        {
            Overall.CleanValue += rawValue;
            Docker.CalculationBuffer.Add(Id);
        }

        public void UpdateOverall(ref ParameterRawValue rawValue)
        {
            for (int i = 0; i < Overalls.length; i++)
            {
                if(Overalls.data[i].Hash != rawValue.Hash)
                    continue;

                Overalls.data[i].CleanValue = rawValue.CleanValue;
                Docker.CalculationBuffer.Add(Id);
                break;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SubtractOverall(float rawValue)
        {
            Overall.CleanValue -= rawValue;
            Docker.CalculationBuffer.Add(Id);
        }

        public void SaveChanges()
        {
            if (Subscribers == null)
                return;

            foreach (var subscriber in Subscribers)
                subscriber.Invoke(this, Docker);
        }
    }
}