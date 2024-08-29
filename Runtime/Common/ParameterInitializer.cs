using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ParameterInitializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(IReadOnlyList<IParameterFactory> factories, IReadOnlyList<IComplexCalculationFormula> formulas)
        {
            var docker = new ParameterDocker(null, factories);
            
            foreach (var factory in factories)
            {
                var baseParameter = factory.CreateParameter(docker);
                var parameter = baseParameter.CreateInstance();
                parameter.SetStaticId(factory.Id);
            }
        }
    }
}