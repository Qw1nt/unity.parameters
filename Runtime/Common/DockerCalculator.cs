using System.Runtime.CompilerServices;
using Parameters.Runtime.Base;
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
                if(docker.CalculationBuffer.length == 0)
                    continue;
                
                foreach (var crateId in docker.CalculationBuffer)
                {
                    Calculate(docker, (ParameterCrateDescriptionBase)docker.GetCrate(crateId));

                    foreach (var child in docker.Children)
                    {
                        if(child.TryGetCrate(crateId, out var childCrate, true) == false)
                            continue;

                        Calculate(child, (ParameterCrateDescriptionBase)childCrate);
                    }
                }

                docker.CalculationBuffer.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Calculate(ParameterDocker docker, ParameterCrateDescriptionBase description)
        {
            description.CleanValue = 0f;

            foreach (var parameter in description.Refs)
                description.CleanValue += parameter.GetValue();

            description.ParentModifiedValue = description.CleanValue;
            
            if(docker.Parent == null)
                return;
            
            if (docker.Parent.TryGetCrate(description.Id, out var parentCrate, true) == true)
                description.ParentModifiedValue += parentCrate.GetValue();
        }
    }
}