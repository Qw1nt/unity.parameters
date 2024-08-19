using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(LifeStealAmountParameter))]
    public partial class LifeStealAmount
    {
    }

    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct LifeStealAmountParameter
    {
    }
}