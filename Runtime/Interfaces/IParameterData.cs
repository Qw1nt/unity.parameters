using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterData
    {
        int Id { get; }
        
        public bool InvertDisplayColors { get; }
        
        int Order { get; }
        
        string DebugName { get; }
        
        public bool IsThisCrateParameter(ComplexParameter complexParameter);
    }
}