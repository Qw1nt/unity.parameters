using System;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Collections;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Extensions
{
    public static class ObservableParameterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable SubscribeOnUpdate(this ObservableParameter observableParameter, Action<ComplexParameterContainer, ComplexParameter> callback)
        {
            TryInitSubscribers(observableParameter.Value);

            var subscriber = new ParameterUpdateSubscriber(observableParameter.Value, callback);
            observableParameter.Value.Subscribers.Add(subscriber);

            return subscriber;
        }

        /*public static IDisposable SubscribeOnUpdate<T>(this ObservableParameter<T> observableParameter, T data, Action<T, ParameterDocker, Parameter> callback) 
            where T : class
        {
            return null;
        }*/

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDisposable SubscribeOnUpdate<T>(this ObservableParameter<T> observableParameter, Action<ComplexParameterContainer, ComplexParameter, T> callback)
            where T : class
        {
            TryInitSubscribers(observableParameter.Value);

            var subscriber = new ParameterUpdateSubscriber<T>(observableParameter.Value, callback, observableParameter.Data);
            observableParameter.Value.Subscribers.Add(subscriber);

            return subscriber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TryInitSubscribers(ComplexParameter complexParameter)
        {
            if (complexParameter.Subscribers == null)
                complexParameter.Subscribers = new SwapList<CrateUpdateSubscriberBase>(1);
        }
    }
}