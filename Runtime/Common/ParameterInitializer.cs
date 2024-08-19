using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ParameterInitializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(IReadOnlyList<IParameterCrateFactory> factories, IReadOnlyList<IComplexCalculationFormula> formulas)
        {
            var docker = new ParameterDocker(null, factories);
            
            foreach (var factory in factories)
            {
                var crateDescription = (ParameterCrateDescriptionBase)factory.CreateCrate(docker);
                var parameter = (ParameterBase) crateDescription.CreateParameter();
                parameter.SetStaticId(factory.Id);
            }
        }
    }
}