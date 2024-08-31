using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Interfaces;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    public class DockerCalculator : IDockerCalculator
    {
        private readonly FastList<ParameterDocker> _dockers;

        public DockerCalculator()
        {
            _dockers = StaticDockerStorage.Dockers;
        }

        public void Update()
        {
            foreach (var docker in _dockers)
                SimpleDockerCalculator.Calculate(docker);
        }
    }
}