using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types.Projectiles
{
    [ParameterCrateDescription(typeof(ProjectileSpeedParameter))]
    public partial class ProjectileSpeed 
    {
    }
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct ProjectileSpeedParameter
    {
    }
}