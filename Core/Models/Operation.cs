using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReportAnalysis.Core.Models
{
    public struct Operation
    {
        public Operation()
        {
            Account = string.Empty;
            DateTime = default(DateTime);
            Amount = default(Money);
            Category = "n/a";
            Description = string.Empty;
        }

        public string Account { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime DateTime { get; set; }

        public Money Amount { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        [JsonIgnore]
        public bool IsUnknownCategory => string.IsNullOrEmpty(Category) || Category == "n/a";

        public Operation WithAccount(string account)
        {
            var operation = this;
            operation.Account = account;
            return operation;
        }

        public Operation WithCategory(string category)
        {
            var operation = this;
            operation.Category = category;
            return operation;
        }
    }

    [JsonConverter(typeof(MoneyConverter))]
    public struct Money
    {
        public Money()
        {
            Value = 0;
            Currency = "";
        }

        public Money(double value, string? currency)
        {
            if (currency == null) throw new ArgumentNullException(nameof(currency));

            Value = value;
            Currency = currency.ToLowerInvariant();
        }

        public static Money FromString(string money)
        {
            var tokens = money.Split(" ");
            return new Money(double.Parse(tokens[0], CultureInfo.InvariantCulture), tokens[1]);
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

        public double TotalRub => _dictionary.Select(pair =>
        {
            switch (pair.Key)
            {
                case "rub":
                    return pair.Value.Value;

                case "try":
                    return pair.Value.Value * 3.66;

                case "amd":
                    return pair.Value.Value * 0.17;

                default:
                    throw new NotImplementedException();
            }
        }).Sum();

        // 02.01.2023
        public double TotalUsd => _dictionary.Select(pair =>
        {
            switch (pair.Key)
            {
                case "rub":
                    return pair.Value.Value * 0.011218898;

                case "try":
                    return pair.Value.Value * 0.033816268;

                case "amd":
                    return pair.Value.Value * 0.0024765437;

                default:
                    throw new NotImplementedException();
            }
        }).Sum();

        public override string ToString()
        {
            return $"{(int)TotalRub} " +
                    string.Join(" ", _dictionary.Values.Select(v => $"{(int)v.Value}{v.Currency}"));
        }
    }

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var token = reader.GetString();
            if (string.IsNullOrEmpty(token)) throw new ParsingException();

            if (token.Length == "dd.MM.yyyy HH:mm".Length)
            {
                return DateTime.ParseExact(token ?? string.Empty, "dd.MM.yyyy HH:mm", null);
            }

            if (token.Length == "dd.MM.yyyy".Length)
            {
                return DateTime.ParseExact(token ?? string.Empty, "dd.MM.yyyy", null);
            }

            throw new ParsingException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd.MM.yyyy HH:mm"));
        }
    }

    public class MoneyConverter : JsonConverter<Money>
    {
        public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var token = reader.GetString();
            if (string.IsNullOrEmpty(token)) throw new ParsingException();

            var tokens = token.Split(' ');
            if (tokens.Length != 2) throw new ParsingException();

            return new Money(double.Parse(tokens[0], CultureInfo.InvariantCulture), tokens[1]);
        }

        public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
        {
            writer.WriteStringValue($"{value.Value} {value.Currency}");
        }
    }
}
