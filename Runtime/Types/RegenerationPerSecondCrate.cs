using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(RegenerationPerSecondParameter))]
    public partial class RegenerationPerSecondCrate 
    {
    }

    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct RegenerationPerSecondParameter
    {
        
    }
}