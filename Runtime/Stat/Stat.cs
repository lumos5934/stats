using System;
using System.Collections.Generic;

namespace LLib
{
    public class Stat
    {
        private readonly List<StatModifier> _modifiers = new();
        private readonly Comparison<StatModifier> _comparison;
        private bool _isDirty = true;
        private float _value;

        public IReadOnlyList<StatModifier> Modifiers => _modifiers;
        public float BaseValue { get; private set; }

        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _value = CalculateValue();
                    _isDirty = false;
                }
                return _value;
            }
        }

        public event Action<float> OnValueChanged;
        
        
        public Stat(float baseValue)
        {
            BaseValue = baseValue;
            _comparison = (a, b) => a.Order.CompareTo(b.Order);
        }
                

        public void SetBaseValue(float baseValue)
        {
            BaseValue = baseValue;
            MarkDirtyAndNotify();
        }

        public void AddModifier(StatModifier mod)
        {
            _modifiers.Add(mod);
            MarkDirtyAndNotify();
        }

        public bool RemoveModifier(Guid modifierId)
        {
            var index = _modifiers.FindIndex(m => m.Id == modifierId);
            if (index < 0) 
                return false;

            _modifiers.RemoveAt(index);
            MarkDirtyAndNotify();
            return true;
        }

        public bool RemoveAllFromSource(object source)
        {
            var numRemovals = _modifiers.RemoveAll(m => m.Source == source);
            if (numRemovals <= 0) 
                return false;

            MarkDirtyAndNotify();
            return true;
        }

        private void MarkDirtyAndNotify()
        {
            _isDirty = true;
            
            OnValueChanged?.Invoke(Value);
        }

        private float CalculateValue()
        {
            var value = BaseValue;
            float percentAddResult = 0;

            _modifiers.Sort(_comparison);

            for (var i = 0; i < _modifiers.Count; i++)
            {
                var mod = _modifiers[i];

                switch (mod.Type)
                {
                    case StatModType.Flat:
                        value += mod.Value;
                        break;

                    case StatModType.PercentAdd:
                        percentAddResult += mod.Value;
                        if (i + 1 >= _modifiers.Count || _modifiers[i + 1].Type != StatModType.PercentAdd)
                        {
                            value *= 1 + percentAddResult;
                            percentAddResult = 0;
                        }
                        break;

                    case StatModType.PercentMult:
                        value *= 1 + mod.Value;
                        break;
                }
            }

            return (float)Math.Round(value, 4);
        }
    }
}