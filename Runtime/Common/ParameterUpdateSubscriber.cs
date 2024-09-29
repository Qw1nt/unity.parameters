using System;
using Parameters.Runtime.Collections;

namespace Parameters.Runtime.Common
{
    internal abstract class CrateUpdateSubscriberBase : IDisposable
    {
        private readonly SwapList<CrateUpdateSubscriberBase> _storage;

        internal CrateUpdateSubscriberBase(ComplexParameter complexParameter)
        {
            _storage = complexParameter.Subscribers;
        }

        internal abstract void Invoke(ComplexParameterContainer container, ComplexParameter complexParameter);

        public void Dispose()
        {
            _storage.Remove(this);
        }
    }

    internal class ParameterUpdateSubscriber : CrateUpdateSubscriberBase
    {
        private readonly Action<ComplexParameterContainer, ComplexParameter> _callback;

        internal ParameterUpdateSubscriber(ComplexParameter complexParameter, Action<ComplexParameterContainer, ComplexParameter> callback) :
            base(complexParameter)
        {
            _callback = callback;
        }

        internal override void Invoke(ComplexParameterContainer container, ComplexParameter complexParameter)
        {
            _callback(container, complexParameter);
        }
    }

    internal class ParameterUpdateSubscriber<TCallbackData> : CrateUpdateSubscriberBase
        where TCallbackData : class
    {
        private readonly Action<ComplexParameterContainer, ComplexParameter, TCallbackData> _callback;
        private readonly TCallbackData _data;

        internal ParameterUpdateSubscriber(ComplexParameter complexParameter, Action<ComplexParameterContainer, ComplexParameter, TCallbackData> callback, TCallbackData data) : base(complexParameter)
        {
            _callback = callback;
            _data = data;
        }

        internal override void Invoke(ComplexParameterContainer container, ComplexParameter complexParameter)
        {
            _callback(container, complexParameter, _data);
        }
    }
}