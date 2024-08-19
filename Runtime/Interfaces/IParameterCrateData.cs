using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterCrateData
    {
        ulong Id { get; }

        CrateType Type { get; }
        
        public bool InvertDisplayColors { get; }
        
        int Order { get; }
        
        string DebugName { get; }
        
        public bool IsThisCrateParameter(IParameterRef parameterRef);
    }
}