using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using Parameters.Runtime.Interfaces;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    public class ParameterDocker
    {
        public readonly IDockerHolder Holder;
        public readonly ParameterDocker Parent;

        // ID крейта -> зависимые параметры
        // private readonly Dictionary<ulong, FastList<ulong>> _cratesDependencies = new(16);
        private readonly Dictionary<ulong, ParameterCrateDescriptionBase> _crates = new(8);
        private readonly Queue<ParameterDocker> _childQueue = new(4);
        private readonly FastList<ParameterDocker> _childBuffer = new(4);
        
        internal readonly FastList<ParameterCrateDescriptionBase> Crates;
        internal readonly FastList<ulong> CalculationBuffer = new();
        
        public readonly FastList<ParameterDocker> Children = new();
        
        public ParameterDocker(IDockerHolder holder, IReadOnlyList<IParameterCrateFactory> cratesData, ParameterDocker parent = null)
        {
            Holder = holder;
            Parent = parent;
            Crates = new FastList<ParameterCrateDescriptionBase>(cratesData.Count);

            foreach (var data in cratesData)
            {
                var crateInstance = data.CreateCrate(this);

                if (crateInstance is ParameterCrateDescriptionBase description == false)
                    continue;

                Crates.Add(description);

                _crates.Add(data.Id, description);
                description.HasChanges = true;
            }

            if(Holder != null)
                StaticDockerStorage.Add(this);
            
            if(Parent == null)
                return;
            
            Parent.AddChild(this);

            foreach (var crate in Crates)
                Parent.CalculationBuffer.Add(crate.Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddChild(ParameterDocker child)
        {
            _childQueue.Clear();
            _childQueue.Enqueue(child);
            
            while (_childQueue.Count > 0)
            {
                var element = _childQueue.Dequeue(); 
                _childBuffer.Add(element);

                foreach (var elementChild in element.Children)
                    _childQueue.Enqueue(elementChild);
            }

            AddChild(_childBuffer);
            _childBuffer.Clear();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddChild(FastList<ParameterDocker> children)
        {
            Children.AddListRange(children);
            Parent?.AddChild(children);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Calculate(ulong crateId)
        {
            if(_crates.ContainsKey(crateId) == false)
                return;
            
            var crate = _crates[crateId];
            var cleanValue = 0f;

            foreach (var parameter in crate.Refs)
                cleanValue += parameter.GetValue();
            
            crate.CleanValue = cleanValue;
            crate.ParentModifiedValue = cleanValue;

            if (Parent?.TryGetCrate(crateId, out var parentCrate) == true)
                crate.ParentModifiedValue += parentCrate.GetValue();
            
            foreach (var child in _children)
            {
                if (child.TryGetCrate(crateId, out var result) == false)
                    continue;
                
                var description = (ParameterCrateDescriptionBase)result;
                description.ParentModifiedValue = crate.CleanValue + description.CleanValue;
            }
        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCrate(ulong id)
        {
            return _crates.ContainsKey(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetCrate(ulong id, out IParameterCrate result)
        {
            result = null;

            if (HasCrate(id) == true)
            {
                result = _crates[id];
                return true;
            }

            if (Parent == null || Parent.HasCrate(id) == false)
                return false;

            result = Parent._crates[id];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParameterCrate GetCrate(ulong crateId)
        {
            if (HasCrate(crateId) == true)
                return _crates[crateId];

            if (Parent != null && Parent.HasCrate(crateId) == true)
                return Parent._crates[crateId];

#if UNITY_EDITOR
            throw new KeyNotFoundException($"Крейт с id {crateId} не найден");
#endif
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParameterCrate GetCrateByParameter(IParameterRef parameterRef)
        {
            foreach (var pair in _crates)
            {
                var crate = pair.Value;

                if (crate.IsParameterBelongs(parameterRef) == false)
                    continue;

                return crate;
            }

            return null;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ApplyCrates(IReadOnlyList<IParameterCrateFactory> crates, ref double value)
        {
            if (crates == null)
                return;

            foreach (var crateData in crates)
            {
#if UNITY_EDITOR
                if (_crates.ContainsKey(crateData.Id) == false)
                    throw new KeyNotFoundException();
#endif

                var crate = _crates[crateData.Id];
                // crate.Apply(ref value);
            }
        }*/
    }
}