using JetBrains.Annotations;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterHolder
    {
        [CanBeNull] IParameterRef GetParameter(ulong crateId);
    }
}