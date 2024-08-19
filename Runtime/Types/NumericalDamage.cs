using Parameters.Runtime.Attributes;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Types
{
    [ParameterCrateDescription(typeof(NumericDamageParameter))]
    public partial class NumericalDamage 
    {
    }
    
    [Parameter(typeof(SingleFloatDescriptor))]
    public partial struct NumericDamageParameter
    {
    }
}