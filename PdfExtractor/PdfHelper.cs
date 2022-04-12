using System;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

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

        public static bool PdfPointEquals(PdfPoint point1, PdfPoint point2)
        {
            return DoublesEquals(point1.X, point2.X) && DoublesEquals(point1.Y, point2.Y);
        }

        public static bool DoublesEquals(double d1, double d2) 
        {
            return d1 + double.Epsilon >= d2 && d1 - double.Epsilon <= d2;
        }
    }
}