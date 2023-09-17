using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReportAnalysis.Core.Models
{
    public class DateRange : IEquatable<DateRange>
    {
        private readonly List<AtomicDateRange> _ranges = new List<AtomicDateRange>();

        public DateRange()
        {
        }

        public DateRange(DateTime from, DateTime to) : this(new AtomicDateRange(from, to))
        {
        }

        private DateRange(AtomicDateRange range)
        {
            _ranges.Add(range);
        }

        public static DateRange Parse(string line) // todo
        {
            return new DateRange(AtomicDateRange.Parse(line));
        }

        public IReadOnlyList<DateRange> Ranges => _ranges.Select(r => new DateRange(r)).ToList();

        public int Year
        {
            get
            {
                var year = _ranges[0].From.Year;
                var all = _ranges.All(r => r.From.Year == year && r.To.Year == year);
                if (!all)
                {
                    throw new InvalidOperationException("Years is inconsistent");
                }
                return year;
            }
        }

        public DateRange Add(DateRange range) // fixme
        {
            var allRanges = _ranges.Concat(range._ranges).ToList();
            var minDate = allRanges.Select(r => r.From).Min();
            var maxDate = allRanges.Select(r => r.To).Max();

            var newDateRange = new DateRange();
            DateTime? currentFrom = minDate;
            DateTime? currentTo = minDate;
            for (var day = minDate; day <= maxDate.AddDays(1); day = day.AddDays(1))
            {
                if (allRanges.Exists(adr => adr.From <= day && adr.To >= day))
                {
                    if (!currentFrom.HasValue)
                    {
                        currentFrom = day;
                    }
                    currentTo = day;
                }
                else
                {
                    if (currentTo.HasValue && currentFrom.HasValue)
                    {
                        newDateRange._ranges.Add(new AtomicDateRange(currentFrom.Value, currentTo.Value));
                        currentFrom = null;
                        currentTo = null;
                    }
                }
            }

            return newDateRange;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as DateRange);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public bool Equals(DateRange? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (_ranges.Count != other._ranges.Count)
            {
                return false;
            }

            for (var i = 0; i < _ranges.Count; i++)
            {
                if (_ranges[i].From != other._ranges[i].From ||
                    _ranges[i].To != other._ranges[i].To)
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return string.Join(",", _ranges.Select(r => r.ToString()));
        }
    }


    internal class AtomicDateRange
    {
        public AtomicDateRange(DateTime from, DateTime to)
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

        public static AtomicDateRange Parse(string input) => new AtomicDateRange(
            DateTime.ParseExact(input.Substring(0, 10), "dd.MM.yyyy", null),
            DateTime.ParseExact(input.Substring(11, 10), "dd.MM.yyyy", null));

        public DateTime From { get; }

        public DateTime To { get; }

        public AtomicDateRange Add(AtomicDateRange range) => new AtomicDateRange(
            From < range.From ? From : range.From,
            To > range.To ? To : range.To
        );

        public override string ToString() => this.Equals(default(AtomicDateRange))
            ? string.Empty
            : From.ToString("dd.MM.yyyy") + "-" + To.ToString("dd.MM.yyyy");
    }
}
