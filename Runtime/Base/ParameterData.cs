using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using UnityEngine;

#if PARAMETERS_UINITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Parameters.Runtime.Base
{
    [CreateAssetMenu(menuName = "Parameters/Parameter Data")]
    public partial class ParameterData : ScriptableObject, IParameterFactory, IParameterStaticIdSetter, IParameterData
    {
        [SerializeField] private Id _id;

        [Space] [SerializeField] private CrateType _type;

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

        public ulong Id => _id;

        public CrateType Type => _type;

#if PARAMETERS_UINITY_LOCALIZATION
        public LocalizedString Measurement => _measurement;
#endif
        public bool InvertDisplayColors => _invertDisplayColors;

        public CrateFormatterBase Formatter => _formatter;

        public int Order => _order;

        public string DebugName => _debugName;

        public FormulaElementDescription[] Formula => null;

        public bool IsThisCrateParameter(Parameter parameterRef)
        {
            return Id == parameterRef.Id;
        }

        public void SetStaticId(ulong id)
        {
            ((IParameterStaticIdSetter)Data).SetStaticId(_id);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parameter CreateParameter(ParameterDocker docker)
        {
            return new Parameter(_id, Formula, docker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parameter CreateParameter(ParameterDocker docker, float defaultValue)
        {
            var parameter = new Parameter(_id, Formula, docker);
            parameter.Add(new ParameterRawValue(defaultValue));

            return parameter;
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