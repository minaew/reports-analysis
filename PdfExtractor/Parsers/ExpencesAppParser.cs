using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using PdfExtractor.Models;

namespace PdfExtractor.Parsers
{
    public class ExpensesAppParser : IParser
    {
        public IEnumerable<Operation> Parse(string path)
        {
            using var stream = File.OpenRead(path);
            var movements = JsonSerializer.Deserialize<Movements>(stream);
            return movements.transactions.Select(t =>
                new Operation
                {
                    Account = movements.label,
                    Amount = new Money(t.amount, movements.currency),
                    DateTime = t.date,
                    Description = t.payee,
                    Category = t.category
                }
            );
        }

        public class Movements
        {
            public string label { get; set; }
            
            public string currency { get; set; }

            public IList<Transaction> transactions { get; set; }
        }

        public class Transaction
        {
            public double amount { get; set; }
            
            [JsonConverter(typeof(DateConverter))]
            public DateTime date { get; set; }
            
            public string payee { get; set; }
            
            public string category { get; set; }
        }

        public class DateConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.ParseExact(reader.GetString(), "dd.MM.yyyy", null);
            }
        
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
