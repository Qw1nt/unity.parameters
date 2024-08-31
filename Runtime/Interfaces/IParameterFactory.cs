using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterFactory
    {
        ulong Id { get; }
        
        Parameter CreateParameter(ParameterDocker docker);

        public FormulaElementDescription[] Formula { get; }
    }
}