using System;
using Scellecs.Collections;

namespace Parameters.Runtime.Common
{
    internal abstract class CrateUpdateSubscriberBase : IDisposable
    {
        private readonly FastList<CrateUpdateSubscriberBase> _storage;

        internal CrateUpdateSubscriberBase(Parameter parameter)
        {
            _storage = parameter.Subscribers;
        }

        internal abstract void Invoke(ParameterDocker docker, Parameter parameter);

        public void Dispose()
        {
            _storage.Remove(this);
        }
    }

    internal class ParameterUpdateSubscriber : CrateUpdateSubscriberBase
    {
        private readonly Action<ParameterDocker, Parameter> _callback;

        internal ParameterUpdateSubscriber(Parameter parameter, Action<ParameterDocker, Parameter> callback) :
            base(parameter)
        {
            _callback = callback;
        }

        internal override void Invoke(ParameterDocker docker, Parameter parameter)
        {
            _callback(docker, parameter);
        }
    }

    internal class ParameterUpdateSubscriber<TCallbackData> : CrateUpdateSubscriberBase
        where TCallbackData : class
    {
        private readonly Action<ParameterDocker, Parameter, TCallbackData> _callback;
        private readonly TCallbackData _data;

        internal ParameterUpdateSubscriber(Parameter parameter,
            Action<ParameterDocker, Parameter, TCallbackData> callback, TCallbackData data) : base(parameter)
        {
            _callback = callback;
            _data = data;
        }

        internal override void Invoke(ParameterDocker docker, Parameter parameter)
        {
            _callback(docker, parameter, _data);
        }
    }
}