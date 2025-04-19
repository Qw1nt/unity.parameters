using Parameters.Runtime.Base;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Extensions;
using Parameters.Runtime.Interfaces;
using TriInspector;
using UnityEngine;

namespace Parameters.Runtime.Common
{
    public class TestEditor : MonoBehaviour
    {
        [SerializeField] private ParameterData1 _pd;
        [SerializeField] private CalculationFormula _formula;
        private IParameterContainerHolder _beh;

        [SerializeField] private float _testValue;

        [Button]
        public void Calc()
        {
            var cont = GetComponent<IParameterContainerHolder>().GetContainer();
            Debug.Log(FormulaExecutor.Calculate(_testValue, _pd.Id, cont, _formula.Descriptions));
        }

        [Button]
        public void Prepare()
        {
            _formula.Prepare(_pd);
        }
    }
}