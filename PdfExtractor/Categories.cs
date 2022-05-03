using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PdfExtractor.Models;

namespace PdfExtractor
{
    public class Categories
    {
        private readonly IDictionary<string, ICollection<string>>? _tree;

        public Categories(string filePath)
        {
            var content = File.ReadAllText(filePath);
            _tree = JsonSerializer.Deserialize<IDictionary<string, ICollection<string>>>(content);
            if (_tree == null)
            {
                throw new ArgumentException("invalid categories file content", filePath);
            }
        }

        public string GetCategory(Operation operation)
        {
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
}
