using System;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Base
{
    public abstract class ParameterBase : IParameterRef
    {
        private object _callback;
        private object _callbackTarget;
        protected Action<ParameterBase> ChangesEmitter;

        public abstract ulong ParameterId { get; }

        internal abstract void SetStaticId(ulong id);

        public abstract void SetValue(double value);
        
        public abstract float GetValue();

        internal void SetChangeCallback(ParameterCrateDescriptionBase crateDescriptionBase, Action<ParameterCrateDescriptionBase> callback)
        {
            _callback = callback;
            _callbackTarget = crateDescriptionBase;
            
            ChangesEmitter = parameterRef =>
            {
                var typedCallback = (Action<ParameterCrateDescriptionBase>) parameterRef._callback;
                var typedCallbackTarget = (ParameterCrateDescriptionBase)parameterRef._callbackTarget;
                typedCallback.Invoke(typedCallbackTarget);
            };
        }
    }
}