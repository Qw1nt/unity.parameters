using System;
using System.Collections.Generic;
using Parameters.Runtime.Base;

namespace Parameters.Runtime
{
    public static class ParametersCratesStorage
    {
        private static readonly Dictionary<int, ParameterData1> Storage = new();

        public static void InitData(IList<ParameterData1> items)
        {
            Storage.Clear();

            foreach (var item in items)
                Storage.Add(item.Id, item);
        }

        public static ParameterData1 Get(int id)
        {
#if UNITY_EDITOR
            if (Storage.ContainsKey(id) == false)
                throw new ArgumentException("ADD DESC");
#endif

            return Storage[id];
        }
    }
}