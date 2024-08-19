using UnityEngine;

namespace Parameters.Runtime.Common
{
    public class DockerCalculatorMonoProvider : MonoBehaviour
    {
        private DockerCalculator _dockerCalculator;
        
        private void Awake()
        {
            _dockerCalculator = new DockerCalculator();
        }

        private void Update()
        {
            _dockerCalculator.Update();
        }
    }
}