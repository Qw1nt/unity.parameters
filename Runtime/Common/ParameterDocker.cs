using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using Parameters.Runtime.Interfaces;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    public class ParameterDocker : IDisposable
    {
        public readonly IDockerHolder Holder;
        public readonly ParameterDocker Parent;

        private readonly Dictionary<ulong, ParameterCrateDescriptionBase> _crates = new(8);
        private readonly Queue<ParameterDocker> _childQueue = new(4);
        private readonly FastList<ParameterDocker> _childBuffer = new(4);

        internal readonly FastList<ParameterCrateDescriptionBase> Crates;
        internal readonly FastList<ulong> CalculationBuffer = new();

        public readonly FastList<ParameterDocker> Children = new();

        public ParameterDocker(IDockerHolder holder, IReadOnlyList<IParameterCrateFactory> cratesData,
            ParameterDocker parent = null)
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

            if (Holder != null)
                StaticDockerStorage.Add(this);

            if (Parent == null)
                return;

            Parent.AddChild(this);

            foreach (var crate in Crates)
                CalculationBuffer.Add(crate.Id);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveChild(ParameterDocker child)
        {
            var children = child.Children;
            Children.RemoveSwap(child, out _);
            
            var length = children.length;

            for (int i = 0; i < length; i++)
                Children.RemoveSwap(children.data[i], out _);
            
            Parent?.RemoveChild(child);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCrate(ulong id)
        {
            return _crates.ContainsKey(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetCrate(ulong id, out IParameterCrate result, bool onlyInSelf = false)
        {
            result = null;

            if (HasCrate(id) == true)
            {
                result = _crates[id];
                return true;
            }

            if (onlyInSelf == true)
                return false;
            
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

        public void Dispose()
        {
            Parent?.RemoveChild(this);
            StaticDockerStorage.Remove(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ParameterDocker GetPlayerDocker()
        {
            return StaticDockerStorage.PlayerDocker;
        }
    }
}