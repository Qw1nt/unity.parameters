using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(PickUpRadiusParameter))]
    public partial class PickUpRadiusCrate
    {
    }
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct PickUpRadiusParameter
    {
        
    }
}