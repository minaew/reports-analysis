using System;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig.Content;

namespace PdfExtractor
{
    internal class PdfHelper
    {
        public static OpenRect? GetRect(IReadOnlyList<Word> words, params string[] text)
        {
            for (var i = 0; i < words.Count - text.Length; i++)
            {
                var isEqual = true;
                for (var j = 0; j < text.Length; j++)
                {
                    if (text[j] != words[i+j].Text)
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                {
                    // assume first is very left, last is very right, first top is very top
                    return new OpenRect
                    {
                        Left = words[i].BoundingBox.Left,
                        Top = words[i].BoundingBox.Top,
                        Right = words[i+text.Length-1].BoundingBox.Right
                    };
                }
            }

            return null;
        }

        public static IEnumerable<Tuple<double, string>> GetLines(IEnumerable<Word> words)
        {
            foreach (var group in words.GroupBy(d => d.BoundingBox.Centroid.Y))
            {
                yield return Tuple.Create(group.Key,
                                          string.Join(" ", group.Select(g => g.Text)));
            }
        }
    }
}