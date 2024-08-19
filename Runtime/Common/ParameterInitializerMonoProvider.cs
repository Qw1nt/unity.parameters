using Parameters.Runtime.Base;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    [DefaultExecutionOrder(-1000)]
    public class ParameterInitializerMonoProvider : MonoBehaviour
    {
        [SerializeField] private ParameterCrateData[] _crates;
        [SerializeField] private CalculationFormulaDataBase[] _formulas;
        
        private void Awake()
        {
            ParameterInitializer.Initialize(_crates, _formulas);
        }
    }
}