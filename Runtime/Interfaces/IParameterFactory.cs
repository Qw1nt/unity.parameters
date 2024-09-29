using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterFactory
    {
        ulong Id { get; }
        
        ComplexParameter CreateParameter(ComplexParameterContainer container);

        FormulaElementDescription[] Formula { get; }
    }
}