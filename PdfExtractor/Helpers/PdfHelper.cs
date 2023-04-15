using System;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig;
using PdfExtractor.Models;

namespace PdfExtractor.Helpers
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
                    if (text[j] != words[i + j].Text)
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
                        Right = words[i + text.Length - 1].BoundingBox.Right
                    };
                }
            }

            return null;
        }

        public static OpenRect? GetHeaderRectangle(Page page, string headerWords)
        {
            var words = page.GetWords().ToList();
            var rect = GetRect(words, headerWords.Split(' '));
            if (!rect.HasValue)
            {
                return null;
            }
            var headerRect = rect.Value;

            var rects = page.ExperimentalAccess.Paths.Select(p => p.GetBoundingRectangle())
                                                     .Where(r => r.HasValue)
                                                     .Cast<PdfRectangle>()
                                                     .Where(r => r.Centroid.Y < headerRect.Top)
                                                     .ToList();

            var rects2 = rects.Where(r => r.Left <= headerRect.Left + 1 &&
                                          r.Right >= headerRect.Right - 1)
                              .ToList();

            var rect2 = rects2.First();

            return new OpenRect
            {
                Left = rect2.Left,
                Top = rect2.Top,
                Right = rect2.Right
            };
        }

        public static IEnumerable<Tuple<double, string>> GetLines(IEnumerable<Word> words)
        {
            foreach (var group in words.GroupBy(d => d.BoundingBox.Bottom))
            {
                yield return Tuple.Create(group.Key,
                                          string.Join(" ", group.Select(g => g.Text)));
            }
        }

        public static IEnumerable<string> GetLinesByBottom(IEnumerable<Word> words)
        {
            foreach (var group in words.GroupBy(d => d.BoundingBox.Bottom))
            {
                yield return string.Join(" ", group.Select(g => g.Text));
            }
        }

        public static IEnumerable<Word> Split(IEnumerable<Word> words)
        {
            return words.Select(Split).SelectMany(c => c);
        }

        public static IEnumerable<Word> Split(Word word)
        {
            var letters = new List<Letter>();
            foreach (var letter in word.Letters)
            {
                if (letters.Count == 0)
                {
                    letters.Add(letter);
                }
                else
                {
                    var lastLetter = letters[^1];

                    if (!PdfPointEquals(lastLetter.EndBaseLine, letter.StartBaseLine))
                    {
                        yield return new Word(letters);
                        letters = new List<Letter>();
                    }

                    letters.Add(letter);
                }
            }

            yield return new Word(letters);
        }

        private static bool PdfPointEquals(PdfPoint point1, PdfPoint point2)
        {
            return DoublesEquals(point1.X, point2.X) && DoublesEquals(point1.Y, point2.Y);
        }

        public static bool DoublesEquals(double d1, double d2)
        {
            return d1 + double.Epsilon >= d2 && d1 - double.Epsilon <= d2;
        }

        public static bool ContainsSequential(string path, params string[] sequence)
        {
            var words = GetWords(path);
            for (var i = 0; i < words.Count - sequence.Length; i++)
            {
                var equals = true;
                for (var j = 0; j < sequence.Length; j++)
                {
                    if (words[i + j].Text != sequence[j])
                    {
                        equals = false;
                        break;
                    }
                }
                if (equals)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFirstWord(string path) => GetWords(path).FirstOrDefault()?.Text ?? string.Empty;

        public static IReadOnlyList<string> GetContent(string path)
        {
            using var document = PdfDocument.Open(path);

            return document.GetPages()
                .Select(page => page.GetWords())
                .Select(words => PdfHelper.GetLines(words)
                                          .Select(w => w.Item2))
                .SelectMany(lines => lines)
                .ToList();
        }

        public static IReadOnlyList<Word> GetWords(string path)
        {
            using var document = PdfDocument.Open(path);
            return document.GetPages().SelectMany(p => p.GetWords()).ToList();
        }

        public static string Join(IEnumerable<Word> words, string separator = " ")
        {
            return string.Join(" ", words.Select(w => w.Text));
        }
    }
}