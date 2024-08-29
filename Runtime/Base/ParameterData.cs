using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using UnityEngine;

namespace Parameters.Runtime.Base
{
    [CreateAssetMenu(menuName = "Parameters/Crate Data")]
    public partial class ParameterData : ScriptableObject, IParameterFactory, IParameterData
    {
        [SerializeField] private Id _id;

        [Space] [SerializeField] private CrateType _type;

#if PARAMETERS_UINITY_LOCALIZATION
        [Space] [SerializeField] private LocalizedString _name;
        [SerializeField] private LocalizedString _measurement;
#endif
        [Space] [SerializeField] private bool _invertDisplayColors;
        // [SerializeField] private ParameterCrateData[] _influentialCrates;

        [Space] [SerializeField] private CrateFormatterBase _formatter;
        [SerializeField] private int _order;

        [Space] [SerializeField] private string _debugName;
        [SerializeReference] [HideInInspector] public object Data;

        protected const string BaseMenuName = "Parameters/Crates/";

        public ulong Id => _id;

        public CrateType Type => _type;

#if PARAMETERS_UINITY_LOCALIZATION
        public LocalizedString Measurement => _measurement;
#endif
        public bool InvertDisplayColors => _invertDisplayColors;

        // public IReadOnlyList<IParameterFactory> InfluentialCrates => _influentialCrates;

        public CrateFormatterBase Formatter => _formatter;

        public int Order => _order;

        public string DebugName => _debugName;

        public bool IsThisCrateParameter(Parameter parameterRef)
        {
            return Id == parameterRef.Id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parameter CreateParameter(ParameterDocker docker)
        {
            return ((Parameter)Data).CreateInstance(_id, _type, docker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parameter CreateParameter()
        {
            return ((Parameter)Data).CreateInstance();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Parameter CreateParameter(float defaultValue)
        {
            return ((Parameter)Data).CreateInstance(defaultValue);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDefaultValue(Parameter parameter, float defaultValue)
        {
            parameter.Add(CreateParameter(defaultValue));
        }*/

#if PARAMETERS_UINITY_LOCALIZATION && PARAMETERS_UNITASK
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<string> GetName()
        {
            return await _name.GetLocalizedStringAsync();
        }
#endif
    }
}