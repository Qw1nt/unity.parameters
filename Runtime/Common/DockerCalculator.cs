using System.Runtime.CompilerServices;
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
            {
                if (docker.CalculationBuffer.length == 0)
                    continue;

                foreach (var crateId in docker.CalculationBuffer)
                {
                    Calculate(docker, docker.GetParameter(crateId));

                    foreach (var child in docker.Children)
                    {
                        if (child.TryGetCrate(crateId, out var childParameter, true) == false)
                            continue;

                        Calculate(child, childParameter);
                    }
                }

                docker.CalculationBuffer.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Calculate(ParameterDocker docker, Parameter parameter)
        {
            parameter.Value.CleanValue = 0f;
            parameter.Overall.CleanValue = 0f;

            for (int i = 0; i < parameter.Values.length; i++)
                parameter.Value.CleanValue += parameter.Values.data[i].CleanValue;

            for (int i = 0; i < parameter.Overalls.length; i++)
                parameter.Overall.CleanValue += parameter.Overalls.data[i].CleanValue;

            parameter.Value.ParentModifiedValue = parameter.Value.CleanValue;
            parameter.Overall.ParentModifiedValue = parameter.Overall.CleanValue;

            if (docker.Parent == null)
                return;

            if (docker.Parent.TryGetCrate(parameter.Id, out var parentParameter, true) == false)
                return;

            parameter.Value.ParentModifiedValue += parentParameter.Value.ParentModifiedValue;
            parameter.Overall.ParentModifiedValue += parentParameter.Overall.ParentModifiedValue;
        }
    }
}