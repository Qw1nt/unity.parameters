using Parameters.Runtime.Interfaces;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    public interface IParameterCrate : IReadOnlyParametersCrate
    {
        public ulong Id { get; }
        
        bool HasChanges { get; }
        
        bool IsParameterBelongs(IParameterRef parameterRef);
        
        public void Add(IParameterRef parameterRef);

        public void Remove(IParameterRef parameterRef);
    }

    public class ParameterCrateBase 
    {
        internal FastList<CrateUpdateSubscriberBase> Subscribers;
    }
}