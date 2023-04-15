using System;

namespace PdfExtractor.Models
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

        public DateTime From { get; }

        public DateTime To { get; }

        public DateRange Add(DateRange range) => new DateRange(
            From < range.From ? From : range.From,
            To > range.To ? To : range.To
        );

        public override string ToString() => From.ToString("dd.MM.yyyy") + "-" + To.ToString("dd.MM.yyyy");
    }
}