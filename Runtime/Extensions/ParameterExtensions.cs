using System;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Extensions
{
    public static class ParameterExtensions
    {
        public static ObservableParameter AsObservable(this ComplexParameter complexParameter)
        {
            return new ObservableParameter(complexParameter);
        }      
        
        public static ObservableParameter<T> AsObservable<T>(this ComplexParameter complexParameter, T data) 
            where T : class
        {
            return new ObservableParameter<T>(complexParameter, data);
        }
    }
}