using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(ShotCountCrateParameter))]
    public partial class ShotCountCrate 
    {
    }

    [Parameter(typeof(SingleIntDescriptor))]
    public partial struct ShotCountCrateParameter
    {
        
    }
}