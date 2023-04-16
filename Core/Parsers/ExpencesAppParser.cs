using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReportAnalysis.Core.Interfaces;
using ReportAnalysis.Core.Models;

namespace ReportAnalysis.Core.Parsers
{
    class ExpensesAppParser : IParser, IIdentifier
    {
        public IEnumerable<Operation> Parse(string path)
        {
            using var stream = File.OpenRead(path);
            var movements = JsonSerializer.Deserialize<Movements>(stream);
            if (movements?.transactions == null)
            {
                return Enumerable.Empty<Operation>();
            }

            return movements.transactions.Select(t =>
                new Operation
                {
                    Account = movements.label,
                    Amount = new Money(t.amount, movements?.currency),
                    DateTime = t.date,
                    Description = t.payee,
                    Category = t.category
                }
            );
        }

        public string Identify(string path)
        {
            using var stream = File.OpenRead(path);
            return JsonSerializer.Deserialize<Movements>(stream)?.label ?? throw new InvalidOperationException("error serializing movements");
        }

#pragma warning disable IDE1006 // Naming Styles
        public class Movements
        {
            public string? label { get; set; }

            public string? currency { get; set; }

            public IList<Transaction>? transactions { get; set; }
        }

        public class Transaction
        {
            public double amount { get; set; }

            [JsonConverter(typeof(DateConverter))]
            public DateTime date { get; set; }

            public string? payee { get; set; }

            public string? category { get; set; }
        }
#pragma warning restore IDE1006 // Naming Styles

        public class DateConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.ParseExact(reader.GetString() ?? string.Empty, "dd.MM.yyyy", null);
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
