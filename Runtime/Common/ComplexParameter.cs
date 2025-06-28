using System.Runtime.CompilerServices;
using Parameters.Runtime.CalculationFormulas;
using Parameters.Runtime.Collections;
using Parameters.Runtime.Interfaces;

namespace Parameters.Runtime.Common
{
    public class ComplexParameter : IReadOnlyParameter
    {
        public readonly int Id;

        internal float Flat;
        internal float Percent;

        internal readonly ComplexParameterContainer Container;

        internal FormulaElementDescription[] Formula;
        internal int[] Dependencies;
        
        /// <summary>
        /// Хранит чистое и посчитанное плоские значения параметра
        /// </summary>
        public CalculatedValue CalculatedFlat;
        
        /// <summary>
        /// Хранит чистое и посчитанное процентные значения параметра
        /// </summary>
        public CalculatedValue CalculatedPercent;

        internal FastList<CrateUpdateSubscriberBase> Subscribers;

        internal ComplexParameter(int id, FormulaElementDescription[] formula, int[] dependencies, ComplexParameterContainer container)
        {
            Id = id;

            Formula = formula;
            Dependencies = dependencies;
            Container = container;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryForceRecalculate()
        {
            if (Container.CalculationBuffer.Has(Id) == false)
                return;        
            
            ComplexParameterContainerCalculator.Calculate(Container);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanFlat()
        {
            return CalculatedFlat.CleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetFlat()
        {
            return CalculatedFlat.ParentModifiedValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetCleanPercent()
        {
            return CalculatedPercent.CleanValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetPercent()
        {
            return CalculatedPercent.ParentModifiedValue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFlat(float value)
        {
            Flat += value;

            if(Subscribers != null)
                NotifySubscribers();
            
            if (Container.CalculationBuffer.Has(Id) == true)
                return;
            
            Container.MarkDirty(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFlat(float value)
        {
            Flat -= value;
            Container.MarkDirty(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlat(float value)
        {
            Flat = value;
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
        public void SetPercent(float value)
        {
            Percent = value;
            
            if(Subscribers != null)
                NotifySubscribers();
            
            if (Container.CalculationBuffer.Has(Id) == true)
                return;
            
            Container.MarkDirty(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TryRecalculate()
        {
            if (Container.CalculationBuffer.Has(Id) == false)
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