using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Common;
using Scellecs.Collections;

namespace Parameters.Runtime.Extensions
{
    public static class ObservableParameterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable SubscribeOnUpdate(this ObservableParameter observableParameter,
            Action<ParameterDocker, Parameter> callback)
        {
            TryInitSubscribers(observableParameter.Value);

            var subscriber = new ParameterUpdateSubscriber(observableParameter.Value, callback);
            observableParameter.Value.Subscribers.Add(subscriber);

            return subscriber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable SubscribeOnUpdate<T>(this ObservableParameter<T> observableParameter, Action<ParameterDocker, Parameter, T> callback)
            where T : class
        {
            TryInitSubscribers(observableParameter.Value);

            var subscriber = new ParameterUpdateSubscriber<T>(observableParameter.Value, callback, observableParameter.Data);
            observableParameter.Value.Subscribers.Add(subscriber);

            return subscriber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryInitSubscribers(Parameter parameter)
        {
            if (parameter.Subscribers == null)
                parameter.Subscribers = new FastList<CrateUpdateSubscriberBase>(1);
        }
    }
}