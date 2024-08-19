using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using Scellecs.Collections;

namespace Parameters.Runtime.Base
{
    public abstract class ParameterCrateDescriptionBase : IParameterCrate
    {
        private readonly ParameterDocker _docker;

        internal float CleanValue;
        internal float ParentModifiedValue;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ParameterCrateDescriptionBase()
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected ParameterCrateDescriptionBase(ulong id, ParameterDocker docker)
        {
            Id = id;
            _docker = docker;
        }

        internal FastList<IParameterRef> Refs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        } = new(2);

        public ulong Id
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public bool HasChanges
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set;
        }

        public abstract IParameterRef CreateParameter();
        
        public abstract IParameterRef CreateParameter(double defaultValue);
        
        public abstract IParameterCrate CreateCrate(ulong id, CrateType type, ParameterDocker docker);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetValue()
        {
            return ParentModifiedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanValue()
        {
            return CleanValue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MarkDirty()
        {
            _docker.CalculationBuffer.Add(Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsParameterBelongs(IParameterRef parameterRef)
        {
            return parameterRef.ParameterId == Id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(IParameterRef parameterRef)
        {
            Refs.Add(parameterRef);
            ((ParameterBase)parameterRef).SetChangeCallback(this, self => self.MarkDirty());
            MarkDirty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(IParameterRef parameterRef)
        {
            Refs.Remove(parameterRef);
            MarkDirty();
        }
    }
} 