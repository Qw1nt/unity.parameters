using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(OverallDamageBoostParameter))]
    public partial class OverallDamageBoostCrate
    {
    }   
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct OverallDamageBoostParameter
    {
    }
}