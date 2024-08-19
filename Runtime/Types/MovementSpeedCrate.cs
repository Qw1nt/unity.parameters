using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(MovementSpeedParameter))]
    public partial class MovementSpeedCrate 
    {
    }   
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct MovementSpeedParameter 
    {
    }
}