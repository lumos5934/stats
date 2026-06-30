using System;

namespace LLib
{
    public readonly struct StatModifier : IEquatable<StatModifier>
    {
        public readonly Guid Id;
        public readonly float Value;
        public readonly StatModType Type;
        public readonly int Order;
        public readonly object Source;

        public StatModifier(float value, StatModType type, int order, object source = null)
        {
            Id = Guid.NewGuid();
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public StatModifier(float value, StatModType type, object source = null) : this(value, type, (int)type, source)
        {
        }

        public bool Equals(StatModifier other)
        {
            return Id.Equals(other.Id);
        }
        
        public override bool Equals(object obj)
        {
            return obj is StatModifier other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}