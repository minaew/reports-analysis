using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using PdfExtractor.Models;

namespace PdfExtractor
{
    public interface ICategories
    {
        string GetCategory(Operation operation);
    }

    public class Categories : ICategories
    {
        private readonly IDictionary<string, ICollection<string>>? _tree;
        private readonly IDictionary<DateTime, string> _cases = new Dictionary<DateTime, string>();

        public Categories(string filePath, string specialCasesPath = "")
        {
            var content = File.ReadAllText(filePath);
            _tree = JsonSerializer.Deserialize<IDictionary<string, ICollection<string>>>(content);
            if (_tree == null)
            {
                throw new ArgumentException("invalid categories file content", filePath);
            }

            if (string.IsNullOrEmpty(specialCasesPath))
            {
                return;
            }

            content = File.ReadAllText(specialCasesPath);
            var pairs = JsonSerializer.Deserialize<ICollection<IDictionary<string, string>>>(content);
            if (pairs == null)
            {
                throw new ArgumentException("invalid special cases file content", specialCasesPath);
            }

            _cases = new Dictionary<DateTime, string>(pairs.SelectMany(d => d.Select(p =>
                new KeyValuePair<DateTime, string>(
                    DateTime.ParseExact(p.Key, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
                    p.Value))));
        }

        public string GetCategory(Operation operation)
        {
            if (_cases.TryGetValue(operation.DateTime, out string? cat))
            {
                return cat;
            }

            if (string.IsNullOrEmpty(operation.Description))
            {
                return "n/a";
            }

            foreach (var category in _tree ?? new Dictionary<string, ICollection<string>>())
            {
                // extend here
                if (category.Value.Any(v => operation.Description.Contains(v, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return category.Key;
                }
            }

            return "n/a";
        }
    }

    public class EmptyCategories : ICategories
    {
        public string GetCategory(Operation operation)
        {
            return "n/a";
        }
    }
}
