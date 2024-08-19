using System;
using Parameters.Runtime.Base;
using Parameters.Runtime.Common;
using UnityEngine;

namespace Parameters.Runtime.Formatters
{
    [CreateAssetMenu(menuName = BaseMenuName + "Numeric", fileName = nameof(NumericCrateFormatter))]
    public class NumericCrateFormatter : CrateFormatterBase
    {
        public override double Format(double value)
        {
            return Math.Round(value, 3);
        }

        public override double GetFormattedValue(ParameterDocker docker, ParameterCrateData crateData)
        {
            return Math.Round(docker.GetCrate(crateData.Id).GetValue(), 3);
        }
    }
} 