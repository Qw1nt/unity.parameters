using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using UnityEngine;

#if PARAMETERS_UINITY_LOCALIZATION
using UnityEngine.Localization;
#endif

#if PARAMETERS_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace Parameters.Runtime.Base
{
    [CreateAssetMenu(menuName = "Parameters/Parameter Data")]
    public partial class ParameterData1 : ScriptableObject, IParameterFactory, IParameterStaticIdSetter, IParameterData
    {
        [SerializeField] private Id _id;
        
#if PARAMETERS_UINITY_LOCALIZATION
        [Space] [SerializeField] private LocalizedString _name;
        [SerializeField] private LocalizedString _measurement;
#endif
        [Space] [SerializeField] private bool _invertDisplayColors;

        [Space] [SerializeField] private CrateFormatterBase _formatter;
        [SerializeField] private int _order;

        [Space] [SerializeField] private string _debugName;

        [SerializeField, SerializeReference, HideInInspector]
        public object Data;

        public int Id => _id;
        
#if PARAMETERS_UINITY_LOCALIZATION
        public LocalizedString Measurement => _measurement;
#endif
        public bool InvertDisplayColors => _invertDisplayColors;

        public CrateFormatterBase Formatter => _formatter;

        public int Order => _order;

        public string DebugName => _debugName;

        public FormulaElementDescription[] Formula => null;

        public bool IsThisCrateParameter(ComplexParameter complexParameterRef)
        {
            return Id == complexParameterRef.Id;
        }

        public void SetStaticId(int id)
        {
            ((IParameterStaticIdSetter)Data).SetStaticId(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter CreateParameter(ComplexParameterContainer container)
        {
            return new ComplexParameter(_id, Formula, null, container);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ComplexParameter CreateParameter(ComplexParameterContainer container, FormulaElementDescription[] formula, int[] dependencies)
        {
            return new ComplexParameter(_id, formula, dependencies, container);
        }

#if PARAMETERS_UINITY_LOCALIZATION && PARAMETERS_UNITASK
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<string> GetName()
        {
            return await _name.GetLocalizedStringAsync();
        }
#endif
    }
}