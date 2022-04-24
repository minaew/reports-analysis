using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using PdfExtractor.Models;

namespace PdfExtractor
{
    public class FormatDetector
    {
        public Format GetFormat(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".txt":
                    return Format.RawText;

                case ".pdf":
                    using (var document = PdfDocument.Open(path))
                    {
                        // fixme: performance
                        var allWords = document.GetPages().SelectMany(p => p.GetWords()).ToList();
                         
                        if (ContainsSequencial(allWords, "ДАТА", "ОПЕРАЦИИ", "НАИМЕНОВАНИЕ", "ОПЕРАЦИИ",
                                                    "ШИФР", "СУММА", "ОПЕРАЦИИ", "ОСТАТОК", "НА", "СЧЁТЕ"))
                        {
                            return Format.SberVklad;
                        }
                        if (ContainsSequencial(allWords, "ДАТА", "ОПЕРАЦИИ", "(МСК)", "КАТЕГОРИЯ",
                                                    "СУММА", "В", "ВАЛЮТЕ", "СЧЁТА", "ОСТАТОК", "ПО", "СЧЁТУ"))
                        {
                            return Format.Sber;
                        }
                        if (ContainsSequencial(allWords, "ДАТА", "ОПЕРАЦИИ", "(МСК)", "КАТЕГОРИЯ",
                                                    "СУММА", "В", "ВАЛЮТЕ", "СЧЁТА"))
                        {
                            return Format.Sber;
                        }
                        if (ContainsSequencial(allWords, "Дата", "и", "время", "Дата", "обработки", "Сумма", "Сумма",
                                                    "операции", "банком", "операции", "списания", "Описание"))
                        {
                            return Format.Tinkoff;
                        }

                    }
                    break;
            }            

            throw new InvalidOperationException(path);
        }

        private static bool ContainsSequencial(IReadOnlyList<Word> words, params string[] sequence)
        {
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
    }
}
