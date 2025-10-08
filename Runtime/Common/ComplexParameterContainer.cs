using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Collections;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ComplexParameterContainer : IParameterContainer
    {
        public readonly IParameterContainerHolder Holder;
        
        private readonly ComplexParameterDictionary _map = new(8);
        private readonly ComplexParameterListDictionary _dependenciesMap = new(4); // parameter -> dependents 
        private readonly Queue<ComplexParameterContainer> _childQueue = new(2);
        private readonly FastList<ComplexParameterContainer> _childBuffer = new(2);

        internal readonly FastList<ComplexParameter> Parameters;
        internal readonly IntHashSet CalculationBuffer;
        
        public readonly FastList<ComplexParameterContainer> Children = new();

        private ComplexParameterContainer _parent;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameterContainer(IParameterContainerHolder holder, IReadOnlyList<IParameterFactory> parameters, ComplexParameterContainer parent = null)
        {
            Holder = holder;
            _parent = parent;
            Parameters = new FastList<ComplexParameter>(parameters.Count);
            CalculationBuffer = new IntHashSet(parameters.Count);

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
                        _dependenciesMap.Add(dependent, new FastList<ComplexParameter>(3));
                    
                    _dependenciesMap[dependent].Add(parameter);
                }
            }
            
            if (Holder != null)
                ComplexParameterContainerStorage.Add(this);

            if (_parent == null)
                return;

            _parent.AddChild(this);
        }

        public ComplexParameterContainer Parent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parent;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddChild(ComplexParameterContainer child)
        {
            child._parent = this;
            Children.Add(child);

            //TODO: Мб не работает с:
            
            /*_childQueue.Clear();
            _childQueue.Enqueue(child);

            while (_childQueue.Count > 0)
            {
                var element = _childQueue.Dequeue();
                element._parent = this;

                _childBuffer.Add(element);

                foreach (var elementChild in element.Children)
                    _childQueue.Enqueue(elementChild);
            }

            AddChild(_childBuffer);
            _childBuffer.Clear();*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddChild(FastList<ComplexParameterContainer> children)
        {
            Children.AddRange(children);
            _parent?.AddChild(children);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveChild(ComplexParameterContainer child)
        {
            Children.Remove(child);
            child._parent = null;

            //TODO: Мб не работает с:

            /*var children = child.Children;
            Children.Remove(child);

            var length = children.length;

            for (int i = 0; i < length; i++)
                Children.Remove(children.data[i]);

            _parent?.RemoveChild(child);*/
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
        public bool Has(int id)
        {
            return _map.ContainsKey(id);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter Get(int parameterId)
        {
            if (Has(parameterId) == true)
                return _map[parameterId];

            if (_parent != null && _parent.Has(parameterId) == true)
                return _parent._map[parameterId];

#if UNITY_EDITOR
            throw new KeyNotFoundException($"Параметр с id {parameterId} не найден");
#endif
            return default;
        }        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter SafeGet(int parameterId)
        {
            if (Has(parameterId) == true)
                return _map[parameterId];

            if (_parent != null && _parent.Has(parameterId) == true)
                return _parent._map[parameterId];

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(int id, out ComplexParameter result, bool onlyInSelf = false)
        {
            result = default;

            if (Has(id) == true)
            {
                result = _map[id];
                return true;
            }

            if (onlyInSelf == true)
                return false;
            
            if (_parent == null || _parent.Has(id) == false)
                return false;

            result = _parent._map[id];
            return true;
        }

        public void Dispose()
        {
            _parent?.RemoveChild(this);
            ComplexParameterContainerStorage.Remove(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ComplexParameterContainer GetPlayerDocker()
        {
            return ComplexParameterContainerStorage.PlayerContainer;
        }
    }
}