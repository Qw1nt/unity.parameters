using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using UnityEngine;

namespace Parameters.Runtime.Base
{
    public abstract class CalculationFormulaDataBase : ScriptableObject, IComplexCalculationFormula
    {
        [SerializeField] private ParameterCrateData _targetCrateId;
        // [SerializeField] private FormulaAction[] _actions;
        // [SerializeField] private List<ParameterCrateData> _dependencies;

        // private List<ulong> _dependenciesIds;

        private List<ulong> _dependencies;
        
        protected const string BaseMenuName = "Parameters/Formulas/";
        
        public ulong CalculatedTargetId => _targetCrateId.Id;

        public List<ulong> Dependencies
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (_dependencies == null)
                    _dependencies = BuildDependencies();

                return _dependencies;
            }
        }

        protected abstract List<ulong> BuildDependencies();
        
        public abstract float Execute(ParameterDocker docker);

/*#if UNITY_EDITOR
        [Button]
        private void FindDependencies()
        {
            _dependencies.Clear();
            
            Dictionary<ulong, CalculationFormulaDataBase> formulasMap = new();
            var formulasGuids = UnityEditor.AssetDatabase.FindAssets("t:calculationformuladata");

            foreach (var guid in formulasGuids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var formula = UnityEditor.AssetDatabase.LoadAssetAtPath<CalculationFormulaDataBase>(path);

                formulasMap.Add(formula.CalculatedTargetId, formula);
            }

            Queue<ParameterCrateData> dependencyBuilder = new();

            foreach (var action in _actions)
            {
                dependencyBuilder.Enqueue(action.Left);
                dependencyBuilder.Enqueue(action.Right);
            }

            while (dependencyBuilder.Count > 0)
            {
                var dependency = dependencyBuilder.Dequeue();
                _dependencies.Add(dependency);

                if (formulasMap.TryGetValue(dependency.Id, out var formula) == false || formula._dependencies == null)
                    continue;
                
                foreach (var formulaDependency in formula._dependencies)
                    dependencyBuilder.Enqueue(formulaDependency);
            }
        }
#endif*/

        /*[Serializable]
        public class FormulaAction
        {
            [SerializeField] private ParameterCrateData _left;
            [SerializeField] private ParameterCrateData _right;
            [SerializeField] private CalculationFormulaOperation _operation;

            public ParameterCrateData Left
            {
                get => _left;
            }

            public ParameterCrateData Right
            {
                get => _right;
            }

            public CalculationFormulaOperation Operation
            {
                get => _operation;
            }
        }*/
    }
}