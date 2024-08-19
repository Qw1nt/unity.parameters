using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(PercentsDamageAmountParameter))]
    public partial class PercentsDamageAmount
    {
    }

    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct PercentsDamageAmountParameter
    {
    }
}