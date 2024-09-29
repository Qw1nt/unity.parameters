using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Collections;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ComplexParameter : IReadOnlyParameter
    {
        public readonly ulong Id;

        internal float Flat;
        internal float Percent;

        internal readonly ComplexParameterContainer Container;

        internal FormulaElementDescription[] Formula;
        internal ulong[] Dependencies;
        
        internal CalculatedValue CalculatedFlat;
        internal CalculatedValue CalculatedPercent;

        internal SwapList<CrateUpdateSubscriberBase> Subscribers;

        internal ComplexParameter(ulong id, FormulaElementDescription[] formula, ulong[] dependencies, ComplexParameterContainer container)
        {
            Id = id;

            Formula = formula;
            Dependencies = dependencies;
            Container = container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanFlat()
        {
            TryRecalculate();
            return CalculatedFlat.CleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetFlat()
        {
            TryRecalculate();
            return CalculatedFlat.ParentModifiedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanPercent()
        {
            TryRecalculate();
            return CalculatedPercent.CleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetPercent()
        {
            TryRecalculate();
            return CalculatedPercent.ParentModifiedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFlat(float value)
        {
            Flat += value;
            Container.MarkDirty(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFlat(float value)
        {
            Flat -= value;
            Container.MarkDirty(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPercent(float value)
        {
            Percent += value;
            Container.MarkDirty(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemovePercent(float value)
        {
            Percent -= value;
            Container.MarkDirty(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryRecalculate()
        {
            if (Container.CalculationBuffer.Contains(Id) == false)
                return;

            ComplexParameterContainerCalculator.Calculate(Container);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void NotifySubscribers()
        {
            if (Subscribers == null)
                return;

            foreach (var subscriber in Subscribers)
                subscriber.Invoke(Container, this);
        }
    }
}