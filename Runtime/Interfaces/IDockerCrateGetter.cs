using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IDockerCrateGetter
    {
        float GetValueFromDocker(ParameterDocker docker);
    }
}