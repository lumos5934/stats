using System;
using System.Reflection;
using UnityEngine;

namespace LLib
{
    public class Resource
    {
        public Stat MaxStat { get; }
        public float Current { get; private set; }
        public float Max => MaxStat.Value;

        public event Action<float, float> OnChanged;
        public event Action OnEmpty;

        public Resource(Stat maxStat, float? initialCurrent = null)
        {
            MaxStat = maxStat;
            Current = initialCurrent ?? maxStat.Value;
            
            MaxStat.OnValueChanged += HandleMaxChanged;
        }

        public void Modify(float delta)
        {
            var prev = Current;
            Current = Mathf.Clamp(Current + delta, 0f, Max);

            if (!Mathf.Approximately(Current, prev))
            {
                OnChanged?.Invoke(Current, Max);
            }

            if (Current <= 0f && prev > 0f)
            {
                OnEmpty?.Invoke();
            }
        }

        public void SetCurrent(float value)
        {
            Current = Mathf.Clamp(value, 0f, Max);
            
            OnChanged?.Invoke(Current, Max);
        }

        private void HandleMaxChanged(float newMax)
        {
            Current = Mathf.Min(Current, newMax);
            
            OnChanged?.Invoke(Current, newMax);
        }
    }
}