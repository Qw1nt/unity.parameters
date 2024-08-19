using System;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    internal abstract class CrateUpdateSubscriberBase : IDisposable
    {
        private readonly FastList<CrateUpdateSubscriberBase> _storage;

        internal CrateUpdateSubscriberBase(FastList<CrateUpdateSubscriberBase> storage)
        {
            _storage = storage;
        }
        
        internal abstract void Invoke(IParameterCrate crate, ParameterDocker docker);

        public void Dispose()
        {
            _storage.Remove(this);
        }
    }
    
    internal class CrateUpdateSubscriber : CrateUpdateSubscriberBase
    { 
        private readonly Action<IParameterCrate, ParameterDocker> _callback;
        
        internal CrateUpdateSubscriber(FastList<CrateUpdateSubscriberBase> subscribers, Action<IParameterCrate, ParameterDocker> callback) : base(subscribers)
        {
            _callback = callback;
        }

        internal override void Invoke(IParameterCrate crate, ParameterDocker docker)
        {
            _callback(crate, docker);
        }
    }

    internal class CrateUpdateSubscriber<TCallbackData> : CrateUpdateSubscriberBase
    {
        private readonly Action<IParameterCrate, ParameterDocker, TCallbackData> _callback;
        private readonly TCallbackData _data;
        
        internal CrateUpdateSubscriber(FastList<CrateUpdateSubscriberBase> storage, Action<IParameterCrate, ParameterDocker, TCallbackData> callback, TCallbackData data) : base(storage)
        {
            _callback = callback;
            _data = data;
        }
        
        internal override void Invoke(IParameterCrate crate, ParameterDocker docker)
        {
            _callback(crate, docker, _data);
        }
    }
}