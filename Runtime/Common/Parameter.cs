using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Interfaces;
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
        
        internal FormulaElementDescription[] Formula;

        internal ParameterRawValue Value;
        internal ParameterRawValue Overall;

        internal FastList<CrateUpdateSubscriberBase> Subscribers;

        internal Parameter(ulong id, FormulaElementDescription[] formula, ParameterDocker docker)
        {
            Id = id;
            Formula = formula;
            Docker = docker;
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
        public void Add(ParameterRawValue value, ParameterRawValue overall)
        {
            Values.Add(value);
            Overalls.Add(overall);
            Docker.CalculationBuffer.Add(Id);
            Docker.MarkDependenciesDirty(Id);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(ParameterRawValue rawValue)
        {
            Values.Add(rawValue);
            Docker.CalculationBuffer.Add(Id);
            Docker.MarkDependenciesDirty(Id);
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
                Docker.MarkDependenciesDirty(Id);
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOverall(ParameterRawValue rawValue)
        {
            Overalls.Add(rawValue);
            Docker.CalculationBuffer.Add(Id);
            Docker.MarkDependenciesDirty(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateOverall(ref ParameterRawValue rawValue)
        {
            for (int i = 0; i < Overalls.length; i++)
            {
                if(Overalls.data[i].Hash != rawValue.Hash)
                    continue;

                Overalls.data[i].CleanValue = rawValue.CleanValue;
                Docker.CalculationBuffer.Add(Id);
                Docker.MarkDependenciesDirty(Id);
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SaveChanges()
        {
            if (Subscribers == null)
                return;

            foreach (var subscriber in Subscribers)
                subscriber.Invoke(Docker, this);
        }
    }
}