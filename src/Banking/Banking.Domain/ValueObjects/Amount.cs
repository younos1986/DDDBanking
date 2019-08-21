using System;
using System.Collections.Generic;
using Banking.Domain.Accounts;
using Banking.Domain.SeedWorks;

namespace Banking.Domain.ValueObjects
{
    public class Amount : ValueObject
    {
        public decimal Value {get; private set;}

        public Amount(decimal value)
        {
            Value = value;
        }

        public override string ToString() 
        {
            return Value.ToString("C");
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Value;
        }

        public static implicit operator Amount(decimal v)
        {
            return new Amount(v);
        }

        public static Amount operator *(Amount a, Amount b)
        {
            return new Amount(a.Value * b.Value);
        }
        public static Amount operator +(Amount a , Amount b)
        {
            return new Amount(a.Value + b.Value);
        }

        public static Amount operator -(Amount a, Amount b)
        {
            return new Amount(a.Value - b.Value);
        }

        public static bool operator >(Amount a, Amount b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Amount a, Amount b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(Amount a, Amount b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Amount a, Amount b)
        {
            return a.Value <= b.Value;
        }


    }
}