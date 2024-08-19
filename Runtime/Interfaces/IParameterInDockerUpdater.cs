using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterInDockerUpdater
    {
        public void AddOrUpdateParameter(IParameterCrate crate, IParameterRef parameterCrateDescription, ParameterOperation operation, float value);
    }
}