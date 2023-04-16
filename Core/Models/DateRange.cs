using System;

namespace ReportAnalysis.Core.Models
{
    public struct DateRange
    {
        public DateRange(DateTime from, DateTime to)
        {
            if (from < to)
            {
                From = from;
                To = to;
            }
            else
            {
                From = to;
                To = from;
            }
        }

        public static DateRange Parse(string input) => new DateRange(
            DateTime.ParseExact(input.Substring(0, 10), "dd.MM.yyyy", null),
            DateTime.ParseExact(input.Substring(11, 10), "dd.MM.yyyy", null));

        public DateTime From { get; }

        public DateTime To { get; }

        public DateRange Add(DateRange range) => new DateRange(
            From < range.From ? From : range.From,
            To > range.To ? To : range.To
        );

        public override string ToString() => this.Equals(default(DateRange))
            ? string.Empty
            : From.ToString("dd.MM.yyyy") + "-" + To.ToString("dd.MM.yyyy");
    }
}