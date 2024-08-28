using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterCrateFactory
    {
        ulong Id { get; }
        
        IParameterCrate CreateCrate(ParameterDocker docker);
    }
}