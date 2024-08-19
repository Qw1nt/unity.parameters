using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types.Projectiles
{
    [ParameterCrateDescription(typeof(ProjectileCountParameter))]
    public partial class ProjectileCount
    {
    }
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct ProjectileCountParameter
    {
    }
}