using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(PenetrationParameter))]
    public partial class Penetration
    {
    }
    
    [Parameter(typeof(SingleIntDescriptor))]
    public partial struct PenetrationParameter
    {
    }
}