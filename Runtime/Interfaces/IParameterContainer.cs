using System;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterContainer : IDisposable
    {
        bool Has(ulong id);

        ComplexParameter Get(ulong id);

        bool TryGet(ulong id, out ComplexParameter result, bool onlyInSelf = false);
    }
}