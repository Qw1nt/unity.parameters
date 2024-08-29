using System.Collections.Generic;
using Parameters.Runtime.Attributes;
using Parameters.Runtime.Base;

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