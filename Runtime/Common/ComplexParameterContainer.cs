using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Collections;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ComplexParameterContainer : IParameterContainer
    {
        public readonly IParameterContainerHolder Holder;
        public readonly ComplexParameterContainer Parent;
        
        private readonly Dictionary<ulong, ComplexParameter> _map = new(8);
        private readonly Dictionary<ulong, SwapList<ComplexParameter>> _dependenciesMap = new(4); // parameter -> dependents 
        private readonly Queue<ComplexParameterContainer> _childQueue = new(2);
        private readonly SwapList<ComplexParameterContainer> _childBuffer = new(2);

        internal readonly SwapList<ComplexParameter> Parameters;
        internal readonly HashSet<ulong> CalculationBuffer;

        public readonly SwapList<ComplexParameterContainer> Children = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameterContainer(IParameterContainerHolder holder, IReadOnlyList<IParameterFactory> parameters, ComplexParameterContainer parent = null)
        {
            Holder = holder;
            Parent = parent;
            Parameters = new SwapList<ComplexParameter>(parameters.Count);
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

                foreach (var dependent in parameter.Dependencies)
                {
                    if(_dependenciesMap.ContainsKey(dependent) == false)
                        _dependenciesMap.Add(dependent, new SwapList<ComplexParameter>(4));
                    
                    _dependenciesMap[dependent].Add(parameter);
                }
            }
            
            if (Holder != null)
                ComplexParameterContainerStorage.Add(this);

            if (Parent == null)
                return;

            Parent.AddChild(this);
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddChild(ComplexParameterContainer child)
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
        private void AddChild(SwapList<ComplexParameterContainer> children)
        {
            Children.AddRange(children);
            Parent?.AddChild(children);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RemoveChild(ComplexParameterContainer child)
        {
            var children = child.Children;
            Children.Remove(child);
            
            var length = children.Length;

            for (int i = 0; i < length; i++)
                Children.Remove(children.Items[i]);
            
            Parent?.RemoveChild(child);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MarkDirty(ComplexParameter complexParameter)
        {
            CalculationBuffer.Add(complexParameter.Id);
            complexParameter.NotifySubscribers();

            if (_dependenciesMap.ContainsKey(complexParameter.Id) == false)
                return;

            var dependencies = _dependenciesMap[complexParameter.Id];

            foreach (var dependency in dependencies)
            {
                CalculationBuffer.Add(dependency.Id);
                _map[dependency.Id].NotifySubscribers();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Has(ulong id)
        {
            return _map.ContainsKey(id);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter Get(ulong parameterId)
        {
            if (Has(parameterId) == true)
                return _map[parameterId];

            if (Parent != null && Parent.Has(parameterId) == true)
                return Parent._map[parameterId];

#if UNITY_EDITOR
            throw new KeyNotFoundException($"Параметр с id {parameterId} не найден");
#endif
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(ulong id, out ComplexParameter result, bool onlyInSelf = false)
        {
            result = default;

            if (Has(id) == true)
            {
                result = _map[id];
                return true;
            }

            if (onlyInSelf == true)
                return false;
            
            if (Parent == null || Parent.Has(id) == false)
                return false;

            result = Parent._map[id];
            return true;
        }

        public void Dispose()
        {
            Parent?.RemoveChild(this);
            ComplexParameterContainerStorage.Remove(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexParameterContainer GetPlayerDocker()
        {
            return ComplexParameterContainerStorage.PlayerContainer;
        }
    }
}