using JetBrains.Annotations;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterHolder
    {
        [CanBeNull] Parameter GetParameter(ulong parameterId);
    }
}