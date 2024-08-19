using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;
using Qw1nt.SelfIds.Runtime;
using UnityEngine;

namespace Parameters.Runtime.Base
{
    [CreateAssetMenu(menuName = "Parameters/Crate Data")]
    public partial class ParameterCrateData : ScriptableObject, IParameterCrateFactory, IParameterCrateData
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

        public bool IsThisCrateParameter(IParameterRef parameterRef)
        {
            return Id == parameterRef.ParameterId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParameterCrate CreateCrate(ParameterDocker docker)
        {
            return ((ParameterCrateDescriptionBase)Data).CreateCrate(_id, _type, docker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParameterRef CreateParameter()
        {
            return ((ParameterCrateDescriptionBase)Data).CreateParameter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParameterRef CreateParameter(float defaultValue)
        {
            return ((ParameterCrateDescriptionBase)Data).CreateParameter(defaultValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsThisCrateParameter(ParameterCrateDescriptionBase parameterCrateDescription)
        {
            return parameterCrateDescription.Id == _id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDefaultValue(IParameterCrate crate, float defaultValue)
        {
            crate.Add(CreateParameter(defaultValue));
        }

#if PARAMETERS_UINITY_LOCALIZATION && PARAMETERS_UNITASK
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask<string> GetName()
        {
            return await _name.GetLocalizedStringAsync();
        }
#endif
    }

    /*public abstract class ParameterCrateData<T> : ParameterCrateData
        where T : ParameterBase, IParameter, new()
    {
        public override bool IsThisCrateParameter(ParameterBase parameter)
        {
            return parameter is T;
        }

        public override ParameterBase CreateParameter()
        {
            return new T();
        }

        public override IParameterCrate CreateCrate(ParameterDocker docker)
        {
            switch (Type)
            {
                case CrateType.Numeric:
                    return this.CreateDefault(docker);

                case CrateType.Percent:
                    return this.CreatePercent(docker);
            }

            return null;
        }

        public override void AddDefaultValue(IParameterCrate crate, float defaultValue)
        {
            var parameter = new T
            {
                Value = defaultValue
            };

            crate.Add(parameter);
        }

        protected Stash<TParameter> GetParameterStash<TParameter>(World world, Entity entity)
            where TParameter : struct, IComponent
        {
            var stash = world.GetStash<TParameter>();

            if (stash.Has(entity) == false)
                stash.Add(entity);

            return stash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IParameterCrate<T> GetFromDocker(ParameterDocker docker)
        {
            return docker.GetCrate<T>(Id);
        }
    }*/

    /*public static class ParameterCrateDataExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParameterCrate CreateDefault<T>(this ParameterCrateData<T> parameterCrateData,
            ParameterDocker docker)
            where T : ParameterBase, new()
        {
            return new ParameterCrate<T>(parameterCrateData.Id, parameterCrateData.Type, parameterCrateData.InfluentialCrates, docker);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParameterCrate CreatePercent<T>(this ParameterCrateData<T> parameterCrateData,
            ParameterDocker docker)
            where T : ParameterBase, new()
        {
            return new PercentParameterCrate<T>(parameterCrateData.Id, parameterCrateData.Type, parameterCrateData.InfluentialCrates, docker);
        }
    }*/
}