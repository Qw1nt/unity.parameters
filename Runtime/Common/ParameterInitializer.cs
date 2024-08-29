using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ParameterInitializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(IReadOnlyList<IParameterStaticIdSetter> setters)
        {
            foreach (var setter in setters)
                setter.SetStaticId(setter.Id);
        }
    }
}