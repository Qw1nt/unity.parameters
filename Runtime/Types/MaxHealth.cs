using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(MaxHealthParameter))]
    public partial class MaxHealth
    {

    }    
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct MaxHealthParameter
    {

    }
}