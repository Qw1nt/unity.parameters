using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Interfaces;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    public class ParameterDocker : IDisposable
    {
        public readonly IDockerHolder Holder;
        public readonly ParameterDocker Parent;
        
        private readonly Dictionary<ulong, Parameter> _map = new(8);
        private readonly Dictionary<ulong, FastList<Parameter>> _dependencies = new(4); 
        private readonly Queue<ParameterDocker> _childQueue = new(2);
        private readonly FastList<ParameterDocker> _childBuffer = new(2);

        internal readonly FastList<Parameter> Parameters;
        internal readonly HashSet<ulong> CalculationBuffer;

        public readonly FastList<ParameterDocker> Children = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ParameterDocker(IDockerHolder holder, IReadOnlyList<IParameterFactory> parameters, ParameterDocker parent = null)
        {
            Holder = holder;
            Parent = parent;
            Parameters = new FastList<Parameter>(parameters.Count);
            CalculationBuffer = new HashSet<ulong>(parameters.Count);

            foreach (var data in parameters)
            {
                var instance = data.CreateParameter(this);

                instance.Formula = data.Formula;
                
                _map.Add(instance.Id, instance);
                Parameters.Add(instance);
                CalculationBuffer.Add(instance.Id);
            }

            foreach (var parameter in Parameters)
            {
                if(parameter.Formula == null || parameter.Formula.Length == 0)
                    continue;
                
                foreach (var element in parameter.Formula)
                {
                    if (element.LeftSource == FormulaDataSource.Parameter)
                        AddDependency(parameter.Id, element.LeftParameterId);
                    
                    if(element.RightSource == FormulaDataSource.Parameter)
                        AddDependency(parameter.Id, element.RightParameterId);
                }
            }
            
            if (Holder != null)
                StaticDockerStorage.Add(this);

            if (Parent == null)
                return;

            Parent.AddChild(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddDependency(ulong calculatedId, ulong dependedId)
        {
            if(_dependencies.ContainsKey(dependedId) == false)
                _dependencies.Add(dependedId, new FastList<Parameter>(2));
            
            _dependencies[dependedId].Add(_map[calculatedId]);
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
        internal void MarkDependenciesDirty(ulong id)
        {
            if(_dependencies.ContainsKey(id) == false)
                return;

            var dependencies = _dependencies[id];

            foreach (var dependency in dependencies)
                CalculationBuffer.Add(dependency.Id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasCrate(ulong id)
        {
            return _map.ContainsKey(id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetCrate(ulong id, out Parameter result, bool onlyInSelf = false)
        {
            result = null;

            if (HasCrate(id) == true)
            {
                result = _map[id];
                return true;
            }

            if (onlyInSelf == true)
                return false;
            
            if (Parent == null || Parent.HasCrate(id) == false)
                return false;

            result = Parent._map[id];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parameter GetParameter(ulong parameterId)
        {
            if (HasCrate(parameterId) == true)
                return _map[parameterId];

            if (Parent != null && Parent.HasCrate(parameterId) == true)
                return Parent._map[parameterId];

#if UNITY_EDITOR
            throw new KeyNotFoundException($"Параметр с id {parameterId} не найден");
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