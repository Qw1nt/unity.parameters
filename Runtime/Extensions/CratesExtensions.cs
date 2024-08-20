using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Extensions
{
    public static class CratesExtensions
    {
#if PARAMETERS_UINITY_LOCALIZATION && PARAMETERS_UNITASK
        public static async UniTask<string> FormatMeasurement(this ParameterCrateData crateData, List<object> arguments)
        {
            return await crateData.Measurement.GetLocalizedStringAsync(arguments);
        }
#endif
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IParameterCrate GetCrateByParameter(this ParameterDocker docker, IParameterRef parameterRef)
        {
            foreach (var crate in docker.Crates)
            {
                if (crate.IsParameterBelongs(parameterRef) == false)
                    continue;

                return crate;
            }

            return null;
        }
    }
}