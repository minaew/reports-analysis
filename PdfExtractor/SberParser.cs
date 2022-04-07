using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PdfExtractor
{
    public class SberParser : IParser
    {
        // e.g.: 29.09.202110:15
        private readonly Regex _dateRegex = new(@"\d{2}\.\d{2}\.\d{4}", RegexOptions.Compiled);
        private readonly Regex _timeRegex = new(@"\d{2}:\d{2}", RegexOptions.Compiled);

        public IReadOnlyList<Operation> Parse(string path)
        {
            var operations = new List<Operation>();

            using (var document = PdfDocument.Open(path))
            {
                foreach (var page in document.GetPages())
                {
                    var rowWords = new List<Word>();
                    var words = SplitY(page.GetWords()).ToList();
                    var isInsizeRow = false;
                    for (var i = 0; i < words.Count - 1; i++)
                    {
                        var word = words[i].Text;
                        var nextWord = words[i+1].Text;
                        if (_dateRegex.IsMatch(word) && _timeRegex.IsMatch(nextWord))
                        {
                            if (isInsizeRow)
                            {
                                var operation = Parse(rowWords);
                                operations.Add(operation);
                                rowWords.Clear();
                            }
                            isInsizeRow = true;
                        }
                        if (isInsizeRow)
                        {
                            rowWords.Add(words[i]);
                        }
                    }

                    // last row
                    if (rowWords.Count > 0)
                    {
                        rowWords.Add(words[^1]);
                        rowWords = RemoveFooter(rowWords).ToList();
                        var lastOperation = Parse(rowWords);
                        operations.Add(lastOperation);
                    }
                }
            }

            return operations;
        }

        public Operation Parse(List<Word> row)
        {
            var dateTime = DateTime.ParseExact(row[0].Text + row[1].Text,
                                               "dd.MM.yyyyHH:mm",
                                               null);

            // find las word in amount expression
            // last word is very previous before second date
            // 1. check there are two instances of date
            var dates = row.Where(w => DateTime.TryParseExact(w.Text, "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime dateTime))
                           .ToList();
            if (dates.Count != 2)
            {
                throw new ParsingException();
            }
            var secondDateIndex = row.IndexOf(dates[1]);
            // 2.
            var endIndex = secondDateIndex - 1;


            var startIndex = -1;
            for (var i = endIndex; i >=0; i--) // backdirected search
            {
                var word = row[i].Text;
                if (word.Any(c => !char.IsDigit(c) && c != '+' && c != ','))
                {
                    startIndex = i + 1;
                    break;
                }
            }
            if (startIndex == -1 || startIndex > endIndex)
            {
                throw new ParsingException("Could not fount amount start word");
            }

            var amountToken = string.Join("", 
                row.Skip(startIndex).Take(endIndex - startIndex + 1));
            amountToken = amountToken.Replace(',', '.');
            if (amountToken.StartsWith('+'))
            {
                amountToken = amountToken[1..];
            }
            else
            {
                amountToken = "-" + amountToken;
            }

            var description = string.Join(" ", row.Skip(secondDateIndex + 2));

            return new Operation
            {
                DateTime = dateTime,
                Amount = double.Parse(amountToken, CultureInfo.InvariantCulture),
                Description = description
            };
        }

        private static IEnumerable<Word> SplitY(IEnumerable<Word> words)
        {
            foreach (var word in words)
            {
                // var newWord = new Word();
                var letters = new List<Letter>();
                for (var i = 0; i < word.Letters.Count; i++)
                {
                    if (i > 0)
                    {
                        if (word.Letters[i].Location.Y != word.Letters[i-1].Location.Y)
                        {
                            // new word
                            yield return new Word(letters);
                            letters.Clear();
                        }
                    }
                    letters.Add(word.Letters[i]);
                }
                yield return new Word(letters);
            }
        }

        private static IEnumerable<Word> RemoveFooter(IReadOnlyList<Word> words)
        {
            for (var i = 0; i < words.Count; i++)
            {
                // fixme: detect y position change
                if (i <= words.Count - 4)
                {
                    if (words[i].Text == "Продолжение" &&
                        words[i+1].Text == "на" &&
                        words[i+2].Text == "следующей" &&
                        words[i+3].Text == "странице")
                    {
                        yield break;
                    }

                    if (words[i].Text == "Реквизиты" &&
                        words[i+1].Text == "для" &&
                        words[i+2].Text == "перевода" &&
                        words[i+3].Text == "на")
                    {
                        yield break;
                    }
                    // todo: support other
                }
                yield return words[i];
            }
        }
    }
}
