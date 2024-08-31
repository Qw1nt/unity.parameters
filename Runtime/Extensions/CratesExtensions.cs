using System.Collections.Generic;
using Parameters.Runtime.Base;

#if PARAMETERS_UINITY_LOCALIZATION && PARAMETERS_UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace Parameters.Runtime.Extensions
{
    public static class CratesExtensions
    {
#if PARAMETERS_UINITY_LOCALIZATION && PARAMETERS_UNITASK
        public static async UniTask<string> FormatMeasurement(this ParameterData crateData, List<object> arguments)
        {
             return await crateData.Measurement.GetLocalizedStringAsync(arguments);
        }
#endif
    }
}