using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(TimeBetweenShotParameter))]
    public partial class TimeBetweenShot
    {
    }
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct TimeBetweenShotParameter
    {
    }
}