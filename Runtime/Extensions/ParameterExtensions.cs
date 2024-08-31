using System;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Extensions
{
    public static class ParameterExtensions
    {
        public static ObservableParameter AsObservable(this Parameter parameter)
        {
            return new ObservableParameter(parameter);
        }      
        
        public static ObservableParameter<T> AsObservable<T>(this Parameter parameter, T data) 
            where T : class
        {
            return new ObservableParameter<T>(parameter, data);
        }
    }
}