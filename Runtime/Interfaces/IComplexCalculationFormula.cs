using System.Collections.Generic;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IComplexCalculationFormula
    {
        ulong CalculatedTargetId { get; }

        List<ulong> Dependencies { get; }
        
        float Execute(ParameterDocker docker);

        /*
        public IComplexCalculationFormula(ulong calculatedTargetId, IReadOnlyList<ulong> dependenciesIds)
        {
            CalculatedTargetId = calculatedTargetId;
            DependenciesIds = new ulong[dependenciesIds.Count];

            for (int i = 0; i < dependenciesIds.Count; i++)
                DependenciesIds[i] = dependenciesIds[i];
        }
        */

        /*
        //TODO сделать
        public float Execute(ParameterDocker docker)
        {
            return 1f;
        }

        public class Operation
        {

        }

        public enum OperationType
        {

        }
    */
    }
}