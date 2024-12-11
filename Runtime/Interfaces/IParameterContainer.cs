using System;
using Parameters.Runtime.Common;

namespace Parameters.Runtime.Interfaces
{
    public interface IParameterContainer : IDisposable
    {
        bool Has(int id);

        ComplexParameter Get(int id);

        bool TryGet(int id, out ComplexParameter result, bool onlyInSelf = false);
    }
}