using Parameters.Runtime.Common;
using UnityEngine;

namespace Parameters.Runtime.Base
{
    public abstract class CrateFormatterBase : ScriptableObject
    {
        protected const string BaseMenuName = "Parameters/Formatters/";

        public abstract double Format(double value);
        
        public abstract double GetFormattedValue(ParameterDocker docker, ParameterCrateData crateData);
    }
}