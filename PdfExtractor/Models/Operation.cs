using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfExtractor.Models
{
    public struct Operation
    {
        public string Account { get; set; }

        public DateTime DateTime { get; set; }

        public Money Amount { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }
    }

    public class Money
    {
        public Money()
        {
        }
        
        public Money(double value, string currency)
        {
            Value = value;
            Currency = currency.ToLowerInvariant();
        }
        
        public static Money FromString(string money)
        {
            var tokens = money.Split(" ");
            return new Money(double.Parse(tokens[0]), tokens[1]);
        }
        
        public double Value { get; set; }
        
        public string Currency { get; set; }
    }

    public class AggregatedMoney
    {
        private readonly IDictionary<string, Money> _dictionary = new Dictionary<string, Money>();

        public AggregatedMoney()
        {
        }

        public AggregatedMoney(Money money)
        {
            Add(money);
        }

        public void Add(Money money)
        {
            if (!_dictionary.ContainsKey(money.Currency))
            {
                _dictionary[money.Currency] = new Money(0, money.Currency);
            }
            
            var value = _dictionary[money.Currency].Value; 
            _dictionary[money.Currency] = new Money(value + money.Value, money.Currency);
        }

        public void Add(AggregatedMoney money)
        {
            foreach (var value in money._dictionary.Values)
            {
                Add(value);
            }
        }

        public override string ToString()
        {
            return string.Join(" ", _dictionary.Values.Select(v => $"{(int)v.Value}{v.Currency}"));
        }
    }
}
